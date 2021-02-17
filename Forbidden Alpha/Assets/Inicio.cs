using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inicio : MonoBehaviour
{
    public SpriteRenderer menu;
    public BoxCollider2D menuHit;
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
