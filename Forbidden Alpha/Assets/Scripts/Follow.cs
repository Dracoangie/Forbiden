using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Clase con la mecanica de seguimiento de los personajes no líderes
/// </summary>
public class Follow : MonoBehaviour
{
    public GameObject character;
    
    //adaptar la actualización del seguimiento a cuando se mueve el personaje
    void Update()
    {
        transform.position = new Vector3(character.transform.position.x, character.transform.position.y, -10);
    }

    //Metodo que inicializa el personaje al que debe seguir
    public void setFollow(GameObject c)
    {
        character = c;
    }
}
