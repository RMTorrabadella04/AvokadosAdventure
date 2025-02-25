using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vida : MonoBehaviour
{

    public Image BarraDeVida;

    public float vidaActual;

    public float vidaMaxima;


    void Start()
    {
  
    }

    // Update is called once per frame
    void Update()
    {
        BarraDeVida.fillAmount = vidaActual / vidaMaxima;

        // Cuando la vida sea 0 Muere

        if (vidaActual <= 0.0f)
        {
            gameObject.GetComponent<MovimientoPersonaje>().Muerte();
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Restando Vida debido al daño de: "+ damage);

        vidaActual = vidaActual - damage;

        Debug.Log("Vida restante: " + vidaActual);
    }
}
