using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Clase para la activacion del menu principal
/// </summary>
public class Inicio : MonoBehaviour
{
    public SpriteRenderer menu;
    public BoxCollider2D menuHit;
    public BoxCollider2D[] hovers;

    /// <summary>
    /// Activacion de las opciones del menu
    /// </summary>
    private void OnMouseDown()
    {
        if (this.GetComponent<SpriteRenderer>().enabled)
        {
            menuHit.enabled = true;
            menu.enabled = true;
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<BoxCollider2D>().enabled = false;
            for(int i = 0; i < hovers.Length; ++i)
            {
                hovers[i].enabled = true;
            }
        }
    }
}
