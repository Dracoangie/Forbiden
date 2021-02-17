using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InicioCierre : MonoBehaviour
{
    public SpriteRenderer btnInicio;
    public BoxCollider2D btnInicioHit;
    public SpriteRenderer btnInicioPulsado;
    public BoxCollider2D btnInicioPulsadoHit;
    public BoxCollider2D[] hovers;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
