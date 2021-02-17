using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apagar : MonoBehaviour
{
    public SpriteRenderer btnHover;
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
