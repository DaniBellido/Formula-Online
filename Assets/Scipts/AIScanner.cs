using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScanner : MonoBehaviour
{
    public float raycastLength = 5f;
  


    // Update is called once per frame
    void Update()
    {
        // Obtener la posición y la dirección del rayo
        Vector3 frontRayOrigin = transform.position;
        Vector3 frontRayDirection = transform.forward;

        // Obtener la posición del rayo en el eje local X del vehículo
        Vector3 rightRayOrigin = transform.position; // Ajustar el origen según la ubicación de tu raycast
        // Establecer la dirección del rayo en el eje local X del vehículo
        Vector3 rightRayDirection = transform.right;

        // Obtener la posición del rayo en el eje local X del vehículo
        Vector3 leftRayOrigin = transform.position; // Ajustar el origen según la ubicación de tu raycast
        // Establecer la dirección del rayo en el eje local X del vehículo
        Vector3 leftRayDirection = -transform.right;



        // Lanzar el raycast en la dirección hacia adelante
        RaycastHit hit;
        if (Physics.Raycast(frontRayOrigin, frontRayDirection, out hit, raycastLength))
        {
            // Comprobar si el objeto golpeado tiene la etiqueta "car"
            if (hit.collider.CompareTag("car"))
            {
                // Mostrar un mensaje de depuración
                Debug.Log("FRONT HIT");

                // Aquí puedes agregar cualquier otra lógica que necesites cuando el rayo golpee un objeto con la etiqueta "car"
            }
        }

        // Dibujar el rayo en el editor de Unity para depuración
        Debug.DrawRay(frontRayOrigin, frontRayDirection * raycastLength, Color.red);

        // Lanzar el raycast a la derecha
        if (Physics.Raycast(rightRayOrigin, rightRayDirection, out hit, 2f))
        {
            // Comprobar si el objeto golpeado tiene el tag "car"
            if (hit.collider.CompareTag("car"))
            {
                Debug.Log("RIGHT HIT");
            }
        }

        // Dibujar el raycast en el editor
        Debug.DrawRay(rightRayOrigin, rightRayDirection * 2f, Color.red);


        // Lanzar el raycast a la izquierda
        if (Physics.Raycast(leftRayOrigin, leftRayDirection, out hit, 2f))
        {
            // Comprobar si el objeto golpeado tiene el tag "car"
            if (hit.collider.CompareTag("car"))
            {
                Debug.Log("LEFT HIT");
            }
        }

        // Dibujar el raycast en el editor
        Debug.DrawRay(leftRayOrigin, leftRayDirection * 2f, Color.red);
    }
}







