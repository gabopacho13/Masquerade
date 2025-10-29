using System.Collections;
using UnityEngine;

public class MaskMovement : MonoBehaviour
{
    public float floatAmplitude = 0.2f; // Amplitud del movimiento de flotación
    public float floatFrequency = 0.8f; // Frecuencia del movimiento de flotación
    private Vector3 initialPosition; // Posición inicial del objeto
    private bool isFloating = false;
    private bool isRotating = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.position; // Guardar la posición inicial
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRotating)
        {
            StartCoroutine(Rotate());
        }
        if (!isFloating)
        {
            StartCoroutine(FloatUpAndDown());
        }
    }

    private IEnumerator Rotate()
    {
        isRotating = true;
        while (true)
        {
            transform.Rotate(Vector3.up, 90.0f * Time.deltaTime);
            yield return null; // Espera un frame antes de continuar
        }
    }

    private IEnumerator FloatUpAndDown()
    {
        isFloating = true;
        while (true)
        {
            float newY = initialPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }
    }
}
