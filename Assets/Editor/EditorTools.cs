using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class EditorTools : Editor
{
    [MenuItem("Tools/Set Box Collider")]
    private static void SetBoxCollider()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject obj in selectedObjects)
        {
            // Obtener o agregar BoxCollider
            BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                boxCollider = obj.AddComponent<BoxCollider>();
                Debug.Log($"Box Collider added to {obj.name}");
            }

            // Obtener todos los Renderers en hijos
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                Debug.LogWarning($"No Renderers found in {obj.name}");
                continue;
            }

            // Calcular bounds combinados en espacio global
            Bounds combinedBounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                combinedBounds.Encapsulate(renderers[i].bounds);
            }

            // Convertir a espacio local del objeto principal
            Vector3 localCenter = obj.transform.InverseTransformPoint(combinedBounds.center);
            Vector3 localSize = combinedBounds.size;
            localSize = new Vector3(
                localSize.x / obj.transform.lossyScale.x,
                localSize.y / obj.transform.lossyScale.y,
                localSize.z / obj.transform.lossyScale.z
            );

            // Asignar al collider
            boxCollider.center = localCenter;
            boxCollider.size = localSize;
        }

        EditorUtility.DisplayDialog("Set Box Collider", "Box Colliders have been set and adjusted for the selected objects.", "OK");
    }
}

public class PropSpawnerWindow : EditorWindow
{
    private GameObject prefabToSpawn;
    private int count = 5;
    private float spacing = 2f;

    private enum Pattern { Line, Circle }
    private Pattern pattern = Pattern.Line;

    private enum Orientation
    {
        None,
        LookAtCenter,
        FaceOutward,
        TangentCW,
        TangentCCW
    }
    private Orientation orientation = Orientation.None;

    [MenuItem("Tools/Prop Spawner")]
    [MenuItem("Window/Prop Spawner")]
    public static void ShowWindow()
    {
        GetWindow<PropSpawnerWindow>("Prop Spawner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Spawner Settings", EditorStyles.boldLabel);

        prefabToSpawn = (GameObject)EditorGUILayout.ObjectField("Prefab to Spawn", prefabToSpawn, typeof(GameObject), false);
        count = EditorGUILayout.IntSlider("Quantity", count, 1, 100);
        spacing = EditorGUILayout.FloatField("Spacing / Radius", spacing);
        pattern = (Pattern)EditorGUILayout.EnumPopup("Pattern", pattern);
        orientation = (Orientation)EditorGUILayout.EnumPopup("Orientation", orientation);

        if (GUILayout.Button("Spawn"))
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("Please assign a prefab to spawn.");
            return;
        }

        if (Selection.activeTransform == null)
        {
            Debug.LogWarning("Please select a GameObject in the scene.");
            return;
        }

        Transform parent = Selection.activeTransform;
        Vector3 spawnCenter = parent.position;

        // Centro visual a partir de bounds de los hijos
        Renderer[] childRenderers = parent.GetComponentsInChildren<Renderer>();
        if (childRenderers.Length > 0)
        {
            Bounds combinedBounds = childRenderers[0].bounds;
            for (int i = 1; i < childRenderers.Length; i++)
                combinedBounds.Encapsulate(childRenderers[i].bounds);
            spawnCenter = combinedBounds.center;
        }

        // Contenedor de props
        string groupName = GameObjectUtility.GetUniqueNameForSibling(parent, "Spawned Props");
        GameObject group = new GameObject(groupName);
        group.transform.SetParent(parent);
        group.transform.localPosition = Vector3.zero;
        Undo.RegisterCreatedObjectUndo(group, "Create Prop Group");

        for (int i = 0; i < count; i++)
        {
            Vector3 positionOffset = Vector3.zero;
            float angle = 0f;

            if (pattern == Pattern.Line)
            {
                positionOffset = new Vector3(i * spacing, 0f, 0f);
            }
            else
            {
                angle = i * Mathf.PI * 2f / count;
                positionOffset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * spacing; // spacing = radio
            }

            // Instancia
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabToSpawn);
            string baseName = prefabToSpawn.name;
            string uniqueName = GameObjectUtility.GetUniqueNameForSibling(group.transform, baseName);
            instance.name = uniqueName;
            instance.transform.SetParent(group.transform, false);

            // Posición
            Vector3 spawnPosition = spawnCenter + positionOffset;
            instance.transform.position = spawnPosition;

            Vector3 forward = instance.transform.forward;
            bool setRotation = false;

            if (orientation != Orientation.None)
            {
                if (pattern == Pattern.Circle)
                {
                    // Vectores útiles
                    Vector3 radialIn = (spawnCenter - spawnPosition).normalized; // hacia el centro
                    Vector3 radialOut = -radialIn;                                 // hacia afuera
                    // Tangentes unitarias en el plano XZ:
                    // CCW: derivada de (cos, sin) -> (-sin, cos)
                    Vector3 tangentCCW = new Vector3(-Mathf.Sin(angle), 0f, Mathf.Cos(angle)).normalized;
                    // CW: opuesto
                    Vector3 tangentCW = new Vector3(Mathf.Sin(angle), 0f, -Mathf.Cos(angle)).normalized;

                    switch (orientation)
                    {
                        case Orientation.LookAtCenter:
                            forward = radialIn;
                            setRotation = true;
                            break;
                        case Orientation.FaceOutward:
                            forward = radialOut;
                            setRotation = true;
                            break;
                        case Orientation.TangentCW:
                            forward = tangentCW;
                            setRotation = true;
                            break;
                        case Orientation.TangentCCW:
                            forward = tangentCCW;
                            setRotation = true;
                            break;
                    }
                }
                else 
                {
                    switch (orientation)
                    {
                        case Orientation.LookAtCenter:
                        case Orientation.FaceOutward:
                            break;
                        case Orientation.TangentCW:
                            forward = Vector3.left;
                            setRotation = true;
                            break;
                        case Orientation.TangentCCW:
                            forward = Vector3.right;
                            setRotation = true;
                            break;
                    }
                }
            }

            if (setRotation && forward.sqrMagnitude > 0.0001f)
            {
                instance.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            }

            Renderer r = instance.GetComponentInChildren<Renderer>();
            if (r != null)
            {
                Bounds b = r.bounds;
                float bottomY = b.min.y;
                instance.transform.position -= new Vector3(0f, bottomY, 0f);
            }

            Undo.RegisterCreatedObjectUndo(instance, "Spawned Prop");
        }
    }
}

