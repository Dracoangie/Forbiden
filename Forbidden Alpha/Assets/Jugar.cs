using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Jugar : MonoBehaviour
{
    public SpriteRenderer menuJugar;
    public int opcion;
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

    private void OnMouseOver()
    {
        menuJugar.enabled = true;
    }

    private void OnMouseExit()
    {
        menuJugar.enabled = false;
    }
}
