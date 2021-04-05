using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Clase para el boton de cierre de aplicacion del menu principal
/// </summary>
public class Apagar : MonoBehaviour
{
    public SpriteRenderer btnHover;

    private void OnMouseDown()
    {
        Application.Quit();
    }
    private void OnMouseOver()
    {
        btnHover.enabled = true;
        this.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnMouseExit()
    {
        btnHover.enabled = false;
        this.GetComponent<SpriteRenderer>().enabled = true;
    }
}
