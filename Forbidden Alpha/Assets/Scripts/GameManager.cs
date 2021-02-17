using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Mapa mapa;
    //public GameObject tilemap;
    // Start is called before the first frame update
    void Start()
    {
          
        mapa.createMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
