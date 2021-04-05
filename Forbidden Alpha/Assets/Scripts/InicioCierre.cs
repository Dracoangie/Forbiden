using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Clase para el funcionamiento de cierre del boton de inicio del menu principal, hay 2 clases para el boton de inicio por que trabaja sobre si estan habilitados
/// </summary>
public class InicioCierre : MonoBehaviour
{
    public SpriteRenderer btnInicio;
    public BoxCollider2D btnInicioHit;
    public SpriteRenderer btnInicioPulsado;
    public BoxCollider2D btnInicioPulsadoHit;
    public BoxCollider2D[] hovers;

    /// <summary>
    /// Desactivacion del menu principal
    /// </summary>
    private void OnMouseDown()
    {
        if (btnInicioPulsado.enabled)
        {
            btnInicioPulsado.enabled = false;
            btnInicioPulsadoHit.enabled = false;
            for (int i = 0; i < hovers.Length; ++i)
            {
                hovers[i].enabled = false;
            }
            btnInicio.enabled = true;
            btnInicioHit.enabled = true;
        }
    }
}
