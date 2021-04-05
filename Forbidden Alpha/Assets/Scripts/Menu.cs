using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Clase de control para el menu principal
/// </summary>
public class Menu : MonoBehaviour
{
    public SpriteRenderer menuJugar;
    public int opcion;

    //Metodo para la activacion de las funciones del menu
    private void OnMouseDown()
    {
        switch (opcion)
        {
            case 1:
                SceneManager.LoadScene("Juego");
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
        
    }

    //Funcionalidad de hover del menu principal
    private void OnMouseOver()
    {
        menuJugar.enabled = true;
    }

    private void OnMouseExit()
    {
        menuJugar.enabled = false;
    }
}
