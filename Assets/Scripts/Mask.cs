using System.Collections;
using UnityEngine;

public class Mask : MonoBehaviour
{
    public float floatAmplitude = 0.2f; // Amplitud del movimiento de flotación
    public float floatFrequency = 0.8f; // Frecuencia del movimiento de flotación
    private Vector3 initialPosition; // Posición inicial del objeto

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.position; // Guardar la posición inicial
        StartCoroutine(FloatUpAndDown()); // Iniciar la flotación
        StartCoroutine(Rotate());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Rotate()
    {
        while(true)
        {
            transform.Rotate(Vector3.up, 90.0f * Time.deltaTime);
            yield return null; // Espera un frame antes de continuar
        }
    }

    private IEnumerator FloatUpAndDown()
    {
        while (true)
        {
            float newY = initialPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }
    }
}
