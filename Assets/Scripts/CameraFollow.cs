using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform player;  // Velocidad de suavizado del movimiento de la c�mara
    public Vector3 offset;  // Distancia entre la c�mara y el jugador

    void LateUpdate()
    {
        // Calcula la posici�n deseada de la c�mara
        transform.position = player.position + offset;
    }
}
