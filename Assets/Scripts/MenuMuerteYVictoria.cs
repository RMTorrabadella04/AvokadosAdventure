using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMuerteYVictoria : MonoBehaviour
{
   public void Volver()
    {
        SceneManager.LoadScene(0);
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(1);
    }
}
