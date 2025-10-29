using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class FireManager : MonoBehaviour
{
    private ParticleSystem ps;
    private Light fireLight;

    public float duration = 2f;
    public bool easeSmoothStep = true;

    private void Start()
    {
        ps = transform.GetComponentInChildren<ParticleSystem>();
        fireLight = transform.GetComponentInChildren<Light>();
    }

    public void StartSwapToFinal()
    {
        var finalColors = new Color[]
        {
            HexToColor("2AFF00"),
            HexToColor("03048E"),
            HexToColor("B400FF"),
        };

        // Color objetivo de la luz: #6100AB
        var lightTarget = HexToColor("6100AB");

        // Corrutina que interpola partículas y luz a la vez
        StartCoroutine(SwapTo(ps, finalColors, duration, fireLight, lightTarget));
    }

    // ---- Corrutina con luz (blend de gradient + color de Light) ----
    private IEnumerator SwapTo(ParticleSystem system, Color[] targetColors, float time, Light lightCmp, Color lightTarget)
    {
        if (system == null) yield break;

        var col = system.colorOverLifetime;
        if (!col.enabled) col.enabled = true;

        // Gradient de partida
        var startMMG = col.color;
        Gradient from = startMMG.mode == ParticleSystemGradientMode.Gradient
                        ? startMMG.gradient
                        : (startMMG.mode == ParticleSystemGradientMode.Color ? OneColorAsGradient(startMMG.color)
                                                                             : FallbackGradient());

        // Gradient objetivo con los mismos "times" que el actual
        Gradient to = BuildTargetGradient(from, targetColors);

        // Color inicial de la luz (si existe)
        Color lightStart = lightCmp ? lightCmp.color : Color.black;

        float t = 0f;
        while (t < time)
        {
            float u = t / time;
            if (easeSmoothStep) u = Mathf.SmoothStep(0f, 1f, u);

            // Interpola el gradient
            Gradient lerped = LerpGradient(from, to, u);
            col.color = new ParticleSystem.MinMaxGradient(lerped);

            // Interpola la luz
            if (lightCmp) lightCmp.color = Color.Lerp(lightStart, lightTarget, u);

            t += Time.deltaTime;
            yield return null;
        }

        // Cierre exacto en el objetivo
        col.color = new ParticleSystem.MinMaxGradient(to);
        if (lightCmp) lightCmp.color = lightTarget;
    }

    // ---- Versión antigua sin luz ----
    private IEnumerator SwapTo(ParticleSystem system, Color[] targetColors, float time)
    {
        if (system == null) yield break;

        var col = system.colorOverLifetime;
        if (!col.enabled) col.enabled = true;

        var startMMG = col.color;
        Gradient from = startMMG.mode == ParticleSystemGradientMode.Gradient
                        ? startMMG.gradient
                        : (startMMG.mode == ParticleSystemGradientMode.Color ? OneColorAsGradient(startMMG.color)
                                                                             : FallbackGradient());

        Gradient to = BuildTargetGradient(from, targetColors);

        float t = 0f;
        while (t < time)
        {
            float u = t / time;
            if (easeSmoothStep) u = Mathf.SmoothStep(0f, 1f, u);

            Gradient lerped = LerpGradient(from, to, u);
            col.color = new ParticleSystem.MinMaxGradient(lerped);

            t += Time.deltaTime;
            yield return null;
        }

        col.color = new ParticleSystem.MinMaxGradient(to);
    }

    // Construye el gradient destino usando los tiempos del gradient plantilla
    private Gradient BuildTargetGradient(Gradient template, Color[] palette)
    {
        var g = new Gradient();

        GradientColorKey[] tKeys = template.colorKeys;
        GradientAlphaKey[] aKeys = template.alphaKeys;

        int n = tKeys.Length;
        GradientColorKey[] cKeys = new GradientColorKey[n];

        // Mapea color[i] al key[i]; si faltan, usa el último color
        for (int i = 0; i < n; i++)
        {
            Color c = palette[Mathf.Clamp(i, 0, palette.Length - 1)];
            cKeys[i] = new GradientColorKey(new Color(c.r, c.g, c.b), tKeys[i].time);
        }

        // Conserva exactamente las alphas originales (tiempos y valores)
        g.SetKeys(cKeys, aKeys);
        return g;
    }

    // Interpola dos gradients A→B creando uno nuevo
    private Gradient LerpGradient(Gradient a, Gradient b, float t)
    {
        // Unión de tiempos de color y alpha de ambos (sin SortedSet por compatibilidad .NET 3.5)
        List<float> times = new List<float>();
        AddTimes(times, a.colorKeys); AddTimes(times, b.colorKeys);
        AddTimes(times, a.alphaKeys); AddTimes(times, b.alphaKeys);
        times.Sort();
        DedupTimesInPlace(times, 1e-3f);

        // Cap de seguridad: máximo 8 keys
        if (times.Count > 8)
        {
            List<float> reduced = new List<float>();
            for (int i = 0; i < 8; i++)
            {
                float u = (i == 7) ? 1f : (i / 7f);
                reduced.Add(Mathf.Lerp(0f, 1f, u));
            }
            times = reduced;
        }

        var cKeys = new GradientColorKey[times.Count];
        var aKeys = new GradientAlphaKey[times.Count];

        for (int i = 0; i < times.Count; i++)
        {
            float x = times[i];
            Color ca = a.Evaluate(x);
            Color cb = b.Evaluate(x);
            Color c = Color.Lerp(ca, cb, t);

            cKeys[i] = new GradientColorKey(new Color(c.r, c.g, c.b), x);
            aKeys[i] = new GradientAlphaKey(c.a, x);
        }

        var g = new Gradient();
        g.SetKeys(cKeys, aKeys);
        return g;
    }

    private static void AddTimes(List<float> dst, GradientColorKey[] keys)
    {
        for (int i = 0; i < keys.Length; i++) dst.Add(keys[i].time);
    }
    private static void AddTimes(List<float> dst, GradientAlphaKey[] keys)
    {
        for (int i = 0; i < keys.Length; i++) dst.Add(keys[i].time);
    }
    private static void DedupTimesInPlace(List<float> times, float eps)
    {
        if (times.Count == 0) return;
        List<float> outT = new List<float>(times.Count);
        float last = times[0];
        outT.Add(last);
        for (int i = 1; i < times.Count; i++)
        {
            float v = times[i];
            if (Mathf.Abs(v - last) > eps)
            {
                outT.Add(v);
                last = v;
            }
        }
        times.Clear();
        times.AddRange(outT);
    }

    // Helpers
    private static Color HexToColor(string hex)
    {
        if (string.IsNullOrEmpty(hex)) return Color.white;
        if (hex[0] != '#') hex = "#" + hex;
        Color c;
        if (ColorUtility.TryParseHtmlString(hex, out c)) return c;
        // Fallback manual (RGB 6 dígitos)
        if (hex.Length >= 7)
        {
            byte r = byte.Parse(hex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
        return Color.white;
    }

    private static Gradient OneColorAsGradient(Color c)
    {
        var g = new Gradient();
        g.SetKeys(
            new[] {
                new GradientColorKey(c, 0f),
                new GradientColorKey(c, 1f)
            },
            new[] {
                new GradientAlphaKey(c.a, 0f),
                new GradientAlphaKey(c.a, 1f)
            }
        );
        return g;
    }

    private static Gradient FallbackGradient()
    {
        var g = new Gradient();
        g.SetKeys(
            new[] {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );
        return g;
    }
}
