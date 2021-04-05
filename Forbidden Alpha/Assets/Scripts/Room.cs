using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Clase Sala
/// </summary>
public class Room
{
    /// <summary>
    /// variables de control de la sala
    /// </summary>
    public static int numRoomsCreated = 0;
    private static int nRoomConn = 0;
    private Vector3Int position;
    private int width;
    private int height;
    private bool connected;
    private bool[] sides;

    //Constructor por defecto
    public Room() { }

    /// <summary>
    /// Constructor de sala con posicion del centro de la sala y su tamaño
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    public Room(Vector3Int pos, int w, int h)
    {
        sides = new bool[4];
        position = pos;
        //Debug.Log("Room position: X->"+position.x+"/Y->"+position.y);
        width = w;
        height = h;
        connected = false;
        ++numRoomsCreated;
    }

    /// <summary>
    /// getter para obtener la posicion del centro de la sala
    /// </summary>
    /// <returns></returns>
    public Vector3Int getRoomCenter()
    {
        return position;
    }

    /// <summary>
    /// Metodo para obtener la posicion del muro en una direccion concreta
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public Vector3Int getRoomPathWall(int dir)
    {
        Vector3Int pos = new Vector3Int(0, 0, 0);
        switch (dir)
        {
            case 0:
                pos.y = position.y + height / 2;
                pos.x = position.x;
                return pos;
            case 1:
                pos.x = position.x + width / 2;
                pos.y = position.y;
                return pos;
            case 2:
                if(height % 2 == 0)
                {
                    pos.y = position.y - height / 2 - 1;
                    pos.x = position.x;
                    return pos;
                }
                else
                {
                    pos.y = position.y - height / 2;
                    pos.x = position.x;
                    return pos;
                }
            case 3:
                if (width % 2 == 0)
                {
                    pos.x = position.x - width / 2 - 1;
                    pos.y = position.y;
                    return pos;
                }
                else
                {
                    pos.x = position.x - width / 2;
                    pos.y = position.y;
                    return pos;
                }
            default:
                pos.z = 1;
                return pos;
        }
    }

    /// <summary>
    /// getter para saber si una sala esta conectada con otra
    /// </summary>
    /// <returns></returns>
    public bool getConnected()
    {
        return connected;
    }
    
    /// <summary>
    /// Metodo general de la clase sala para el control del numero de salas que estan conectadas
    /// </summary>
    public void connect()
    {
        ++nRoomConn;
        connected = true;
    }

    /// <summary>
    /// Metodo para saber si una posicion pertenece a la sala
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool hasPosition(Vector3Int pos)
    {
        int distanceX = 0;
        int distanceY = 0;
        if (height % 2 == 0)
            distanceY = 1;
        if (width % 2 == 0)
            distanceX = 1;
        //Debug.Log("Room 2 : X->"+position.x+"/Y->"+position.y+"  // Position: X->"+pos.x+"/Y->"+pos.y);
        return (((position.y - height/2 - distanceY == pos.y) && ( position.x - width / 2 - distanceX <= pos.x && position.x + width / 2 >= pos.x)) || ((position.y + height / 2 == pos.y) && (position.x - width / 2 - distanceX <= pos.x && position.x + width / 2 >= pos.x)) || ((position.x - width / 2 - distanceX == pos.x) && (position.y - height / 2 - distanceY <= pos.y && position.y + height / 2 >= pos.y)) || ((position.x + width / 2 == pos.x) && (position.y - height / 2 - distanceY <= pos.y && position.y + height / 2 >= pos.y)));
    }

    /// <summary>
    /// Metodo para establecer un lado de la sala como conectado
    /// </summary>
    /// <param name="dir"></param>
    public void setPath(int dir)
    {
        sides[dir] = true;
    }

    /// <summary>
    /// Metodo alternativo para establecer un lado de la sala como conectado para la sala nº2 (la direccion de conexion esta invertida)
    /// </summary>
    /// <param name="dir"></param>
    public void setPath2(int dir)
    {
        switch (dir)
        {
            case 0:
                sides[2] = true;
                break;
            case 1:
                sides[3] = true;
                break;
            case 2:
                sides[0] = true;
                break;
            case 3:
                sides[1] = true;
                break;
        }
    }

    /// <summary>
    /// Metodo que comprueba si se puede crear camino hacia una direccion
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool cantPath(Vector2 direction)
    {
        bool can;
        int dir = 0;
        if (direction.y > 0 && Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
            dir = 2;
        else if (direction.x > 0 && Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            dir = 3;
        else if (direction.x < 0 && Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            dir = 1;
        can = sides[dir];
        return can;
    }
    /// <summary>
    /// metodo que obtiene las posiciones de un muro donde colocar los triggers de sala
    /// </summary>
    /// <param name="side"></param>
    /// <returns></returns>
    private BoundsInt getTriggerPosition(int side)
    {
        int distanceX = 0, distanceY = 0;
        if (width % 2 == 0)
            distanceX = 1;
        if (height % 2 == 0)
            distanceY = 1;

        Vector3Int wallSize;
        Vector3Int pos  = new Vector3Int(position.x - width / 2 - distanceX, position.y - height / 2 - distanceY, 0);

        if (side == 0)
            pos.y += height + ((distanceY != 0)? distanceY : -1);
        else if (side == 1)
            pos.x += width + ((distanceX != 0) ? distanceX : -1);

        if (side % 2 == 0)
            wallSize = new Vector3Int(width + distanceX + ((distanceX!=0)? 1:0), 0, 1);
        else
            wallSize = new Vector3Int(0, height + distanceY + ((distanceY != 0) ? 1 : 0), 1);

        

        return new BoundsInt(pos,wallSize);
    }

    /// <summary>
    /// Metodo para colocar los triggers de sala en las entradas de una sala
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="tilemap"></param>
    public void setTriggers(GameObject tile ,ref Tilemap tilemap)
    {
        GameObject clon = tile;
        for (int i = 0; i < sides.Length; ++i)
        {
            //si el lado tiene camino se comprueba donde colocar el trigger
            if (sides[i]) { 
                BoundsInt wall = getTriggerPosition(i);
                BoundsInt.PositionEnumerator iterator = wall.allPositionsWithin;
                iterator = iterator.GetEnumerator();
                while (iterator.MoveNext())
                {
                    if (tilemap.GetTile(iterator.Current) != null && tilemap.GetTile(iterator.Current).name.Contains("Floor"))
                    {
                        clon.transform.position = tilemap.GetCellCenterWorld(iterator.Current);
                        GameObject.Instantiate(clon);
                    }
                }
            }
        }
    }

}
