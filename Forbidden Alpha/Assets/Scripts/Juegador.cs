using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juegador : MonoBehaviour
{

    public Transform[] seguidores = new Transform[4];
    Vector3 lastPos;
    private Vector3Int tilemapPos;
    //habria que programar que el controlador mandara la orden al jugador de moverse
    private int[,] mapa;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Mover();
    }

    void Mover()
    {
        Vector3 last_pos = transform.position;

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (mapa[(tilemapPos.x+1),tilemapPos.y] != 0)
            {
                lastPos = transform.position;
                transform.position += new Vector3(1, 0, 0);
                tilemapPos += new Vector3Int(1, 0, 0);
                Cola();
            }
            
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (mapa[(tilemapPos.x - 1), tilemapPos.y] != 0)
            {
                lastPos = transform.position;
                transform.position += new Vector3(-1, 0, 0);
                tilemapPos -= new Vector3Int(1, 0, 0);
                Cola();
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (mapa[tilemapPos.x, (tilemapPos.y + 1)] != 0)
            {
                lastPos = transform.position;
                transform.position += new Vector3(0, 1, 0);
                tilemapPos += new Vector3Int(0, 1, 0);
                Cola();
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (mapa[tilemapPos.x, (tilemapPos.y - 1)] != 0)
            {
                lastPos = transform.position;
                transform.position += new Vector3(0, -1, 0);
                tilemapPos -= new Vector3Int(0, 1, 0);
                Cola();
            }
        }




    }

    public void setMapa(int[,] numMap)
    {
        mapa = numMap;
    }
    public void setPos(Vector3Int pos)
    {
        tilemapPos = pos;
    }

    void Cola()
    {
        for (int i = 0; i < seguidores.Length; ++i)
        {
            Vector3 temp = seguidores[i].position;
            seguidores[i].position = lastPos;
            lastPos = temp;
        }


    }
   /* private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "enemigo")
        {
            Combate();
        }
    }

    void Combate()
    {
        bool combat = true;

        do
        {
            if (Input.GetKeyDown(KeyCode.Z))
            { 
                transform.position += new Vector3(0, 1, 0);
            }

        } while (combat == true);
    }*/
}
