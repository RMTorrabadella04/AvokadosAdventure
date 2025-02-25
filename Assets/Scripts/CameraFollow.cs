using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform player;  // Velocidad de suavizado del movimiento de la cámara
    public Vector3 offset;  // Distancia entre la cámara y el jugador

    void LateUpdate()
    {
        // Calcula la posición deseada de la cámara
        transform.position = player.position + offset;
    }
}
