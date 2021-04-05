using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Clase destinada al control de funcionamientos generales del juego jugable
/// </summary>
public class GameManager : MonoBehaviour
{
    public Mapa mapa;
    //public GameObject tilemap;
    // Start is called before the first frame update
    void Start()
    {     
        mapa.createMap();
    }
}
