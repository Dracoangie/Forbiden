using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public static int numRoomsCreated = 0;
    private static int nRoomConn = 0;
    private Vector3Int position;
    private int width;
    private int height;
    private bool connected;
    private bool[] sides;

    public Room() { }
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

    public Vector3Int getRoomCenter()
    {
        return position;
    }

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
    public int getRoomCenterX()
    {
        return position.x;
    }
    public int getRoomCenterY()
    {
        return position.y;
    }

    public bool getConnected()
    {
        return connected;
    }
    
    public void connect()
    {
        ++nRoomConn;
        connected = true;
    }

    public bool hasPosition(Vector3Int pos)
    {
        int distance = 0;
        if (height % 2 == 0 || width % 2 == 0)
            distance = 1;
        //Debug.Log("Room 2 : X->"+position.x+"/Y->"+position.y+"  // Position: X->"+pos.x+"/Y->"+pos.y);
        return (((position.y - height/2 - distance == pos.y) && ( position.x - width / 2 - distance <= pos.x && position.x + width / 2 >= pos.x)) || ((position.y + height / 2 == pos.y) && (position.x - width / 2 - distance <= pos.x && position.x + width / 2 >= pos.x)) || ((position.x - width / 2 - distance == pos.x) && (position.y - height / 2 - distance <= pos.y && position.y + height / 2 >= pos.y)) || ((position.x + width / 2 == pos.x) && (position.y - height / 2 - distance <= pos.y && position.y + height / 2 >= pos.y)));
    }

    public void setPath(int dir)
    {
        sides[dir] = true;
    }

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
    public void paintSides(int i)
    {
        Debug.Log("Room nº: "+i+" / Top Side: "+sides[0]+" / Right Side: "+sides[1]+" / Bottom Side: "+sides[2]+" / Left Side: "+sides[3]);
    }

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
}
