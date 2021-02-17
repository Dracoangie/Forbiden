using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Mapa : MonoBehaviour
{
    private DirectoryInfo tileDir;
    public int minSalas;
    public int maxSalas;
    public int roomWidthMin;
    public int roomWidthMax;
    public int roomHeightMin;
    public int roomHeightMax;
    public int distanceBtwRooms;
    private int numRooms;
    private int MAXANCHO = 50;
    private int MAXALTO = 50;
    public GameObject triggerSala;
    public Transform groupStart;
    public Juegador jugador;

    private struct TileObj
    {
        public string name;
        public Tile tile;
    }
    private Tile[] wallTiles;
    private Tile[] floorTiles;
    private Tile[] cornerTiles;
    private Tile[] turnTiles;
    private Tile environment;
    private TileObj[] tiles;
    private int[,] numMap;
    private Room[] rooms;
    public Tilemap tilemap;
    // Start is called before the first frame update
    public void createMap()
    {
        if (tiles == null)
        {
            getTiles();
        }
        getWallTiles();
        getTurnTiles();
        getFloorTiles();
        getCornerTiles();
        generateMapSize();
        rooms = new Room[numRooms];
        generateMap();
        jugador.setMapa(numMap);
        startRoom();
        generatePaths();
    }


    private void startRoom()
    {
        int room = Random.Range(0, numRooms);
        jugador.setPos(new Vector3Int(rooms[room].getRoomCenterX(), rooms[room].getRoomCenterY(),0));
        groupStart.position = new Vector3(rooms[room].getRoomCenterX(), (rooms[room].getRoomCenterY() + 1f),0);
    }
    private void generateMap()
    {
        for (int i=0;i<MAXANCHO;++i)
        {
            for (int j = 0;j<MAXALTO;++j)
            {
                int makeRoom = Random.Range(0,2);
                Vector3Int currentCell = tilemap.WorldToCell(new Vector3(j, i, 0));
                int width = Random.Range(roomWidthMin, roomWidthMax + 1);
                int height = Random.Range(roomHeightMin, roomHeightMax + 1);
                if (numRooms > 0 && makeRoom == 1 && canRoom(currentCell, width, height))
                {
                    createRoom(currentCell, width, height);
                    --numRooms;
                }
                //else if(tilemap.GetTile(currentCell) == null)
                    //tilemap.SetTile(currentCell, environment);
            }
        }
        
    }

    private void generateMapSize()
    {
        numRooms = Random.Range(minSalas,maxSalas+1);
        rooms = new Room[numRooms];
        numMap = new int[MAXANCHO, MAXALTO];
    }

    private void getTiles()
    {
        int i = 0;
        tileDir = new DirectoryInfo(Application.dataPath+"/Resources");
        FileInfo[] fis = tileDir.GetFiles();
        foreach (FileInfo fi in fis)
        {
            if (fi.Extension.Contains("asset"))
                ++i;     
        }
        tiles = new TileObj[i];
        for (int j=0,z=0;j<fis.Length;++j)
        {
            
            FileInfo f = fis[j];
            //Debug.Log(f.Name);
            //Debug.Log(j);
            if (f.Extension.Contains("asset"))
            {
                tiles[z].name = f.Name;
                tiles[z].tile = Resources.Load(Path.GetFileNameWithoutExtension(f.Name)) as Tile;
                //tiles[z].tile = AssetDatabase.LoadAssetAtPath("Assets/Resources/" + f.Name, typeof(Tile)) as Tile;
                if (f.Name == "TileProto.asset")
                    environment = tiles[z].tile;
                //Debug.Log(tiles[z].tile.name);
                ++z;
            }
        }
    }

    private void createRoom(Vector3Int pos, int w, int h)
    {
        int distance = 0;
        if (w % 2 == 0)
            distance = 1;
        Tile wall;
        Vector3Int position;
        int tileType;
        for (int j = pos.y - h / 2 - distance; j <= pos.y + h / 2; ++j)
        {
            for (int i = pos.x - w / 2 - distance; i <= pos.x + w / 2; ++i)
            {
                position = new Vector3Int(i, j, 0);
                if (j == pos.y - h / 2 - distance || j == pos.y + h / 2 || i == pos.x - w / 2 - distance || i == pos.x + w / 2)
                {
                    //bot wall
                    if (i < pos.x + w / 2 && j == pos.y - h / 2 - distance)
                        tileType = 1;
                    //right wall
                    else if (j < pos.y + h / 2 && i == pos.x + w / 2)
                        tileType = 2;
                    //top wall
                    else if (i > pos.x - w / 2 - distance && j == pos.y + h / 2)
                        tileType = 3;
                    //left wall
                    else
                        tileType = 4;
                    if ((j == pos.y + h / 2 || j == pos.y - h / 2 - distance) && (i == pos.x + w / 2 || i == pos.x - w / 2 - distance))
                    {
                        numMap[i, j] = 3;
                        wall = getCornerTile(tileType);
                    }
                    else
                    {
                        numMap[i, j] = 2;
                        wall = getWallTile(tileType);
                    }
                    tilemap.SetTile(position, wall);
                }
                else
                {
                    numMap[i, j] = 1;
                    tilemap.SetTile(position, getFloorTile());
                }
            }
        }
        addRoom(new Room(pos, w, h));
    }

    private void generatePaths()
    {
        for (int i = 0; i < Room.numRoomsCreated;++i)
        {   
            makePath(ref rooms[i]);
            //rooms[i].paintSides(i);
        }
    }

    private void makePath(ref Room r)
    {
        int[] direction = new int[2];
        Room r2;
        direction = getRoom2Dir(out r2, ref r);
        Vector3Int position = r.getRoomPathWall(direction[0]);

        
        tilemap.SetTile(position, getFloorTile());
        GameObject clon = triggerSala;
        Vector3 posAux = tilemap.CellToWorld(position);
        switch (direction[0])
        {
            case 0:
                posAux.x += 0.65f;
                break;
            case 1:
                posAux.y += 0.65f;
                break;
            case 2:
                posAux.x += 0.65f;
                posAux.y += 1f;
                break;
            case 3:
                posAux.x += 1f;
                posAux.y += 0.65f;
                break;
        }
        clon.transform.position = posAux;
        Instantiate(clon);
        if (position.z == 0)
        {
            //Debug.Log("posicion inicial: (" + position.x + "," + position.y + "," + position.z + ")");
            while (!r.getConnected() && position.x != 0)
            { 
                position = makePathSection(ref direction, position, ref r, ref r2);
            }
            //Debug.Log("Check: X->" + position.x + "/Y->" + position.y);
        }

    }

    private int[] getRoom2Dir(out Room rLink, ref Room r)
    {
        Room r2 = rooms[0];
        Vector2 direction = new Vector2(9999, 9999);
        Vector2 directionAux;
        rLink = new Room();
        int[] dir = new int[2] {0,1};
        for(int i = 1; i < Room.numRoomsCreated; ++i)
        {
            r2 = rooms[i];
            directionAux = new Vector2(r2.getRoomCenterX() - r.getRoomCenterX(), r2.getRoomCenterY() - r.getRoomCenterY());
            if (!r2.cantPath(directionAux) && directionAux.magnitude != 0 && direction.magnitude > directionAux.magnitude)
            {
                direction = directionAux;
                rLink = r2;
            }
        }
        //Debug.Log("Direction: X->" + direction.x + "/Y->" + direction.y);
        if (direction.y < 0) dir[0] = 2;
        if (direction.x < 0) dir[1] = 3;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            swapInts(ref dir);
        }
        //Debug.Log(dir[0]);
        rLink.setPath2(dir[0]);
        //rLink.paintSides(1);
        r.setPath(dir[0]);
        return dir;
    }

    private Vector3Int makePathSection(ref int[] dir, Vector3Int pos, ref Room r, ref Room r2)
    {
        int[,] checks = new int[4,2] { { 0, 1 }, {1,0}, {0,-1}, {-1,0} };
        bool dirChange = false;
        Vector2 posDir = new Vector2(r2.getRoomCenterX() - pos.x, r2.getRoomCenterY() - pos.y);
        Vector2 posAux = new Vector2();
        Vector3Int finalPos = new Vector3Int();
        Vector3Int position;
        for(int i = 0; i < checks.GetLength(0) && !r.getConnected(); ++i)
        {
            position = new Vector3Int(pos.x+checks[i,0],pos.y+checks[i,1],pos.z);
            if (!hasWallTile(position))
            {
                posAux.x = r2.getRoomCenterX() - position.x;
                posAux.y = r2.getRoomCenterY() - position.y;
                if (posAux.magnitude < posDir.magnitude)
                {
                    if (dir[0] != i)
                    {
                        dirChange = true;
                        swapInts(ref dir);
                    }
                    posDir = posAux;
                    finalPos = position;
                }
            }else if (r2.hasPosition(position))
            {
                finalPos = position;
                r.connect();
                tilemap.SetTile(finalPos, getTurnTile(transformDirToCornerTileLeft(dir)));
            }
        }
        /*if (dirChange)
        {
            if ((dir[0] == 0 && dir[1] == 3) || (dir[0] == 1 && dir[1] == 0) || (dir[0] == 2 && dir[1] == 1) || (dir[0] == 3 && dir[1] == 2))
                tilemap.SetTile(finalPos, getTurnTile(transformDirToTurnTileLeft(dir)));
            else
                tilemap.SetTile(finalPos, getCornerTile(transformDirToCornerTileLeft(dir)));
        }
        else
        {
            if(!r.getConnected())
                tilemap.SetTile(finalPos, getWallTile(transformDirToWallTileLeft(dir)));
        }*/
        //Debug.Log("Check: X->"+finalPos.x+"/Y->"+finalPos.y);
        if (finalPos.x != 0)
        {
            tilemap.SetTile(finalPos, getFloorTile());
            numMap[finalPos.x, finalPos.y] = 1;
        }
        //r.connect();
        return finalPos;
        /*bool can = true;
        for (int i = 1; i < 3 && can; ++i)
        {
            switch (dir[0])
            {
                case 0: 
                    pos.y += 1;
                    break;
                case 1:
                    pos.x += 1;
                    break;
                case 2:
                    pos.y -= 1;
                    break;
                case 3:
                    pos.x -= 1;
                    break;
            }
            if (hasWallTile(pos))
            {
                r.connect();
                can = false;
            }
            else
            {
                if(i != 2)
                    tilemap.SetTile(pos, getWallTile(transformDirToWallTileLeft(dir)));
                else
                {
                    if((dir[0] == 0 && dir[1] == 3) || (dir[0] == 1 && dir[1] == 0) || (dir[0] == 2 && dir[1] == 1) || (dir[0] == 3 && dir[1] == 2))
                        tilemap.SetTile(pos, getTurnTile(transformDirToTurnTileLeft(dir)));
                    else
                        tilemap.SetTile(pos, getCornerTile(transformDirToCornerTileLeft(dir)));
                }
            }
        }
        return pos;*/
    }

    private bool canRoom(Vector3Int pos, int w, int h)
    {
        if ((pos.x + w / 2) >= MAXANCHO - 1 || (pos.x - w / 2) <= 1 || (pos.y + h / 2) >= MAXALTO - 1 || (pos.y - h / 2) <= 1) return false;
        bool can = true;
        int distance = 0;
        if (w % 2 == 0 || h % 2 == 0)
            distance = 1;
        for (int i = pos.x - w / 2 - distance - distanceBtwRooms; i <= pos.x + w / 2 + distanceBtwRooms && can; ++i)
        {
            for (int j = pos.y - h / 2 - distance - distanceBtwRooms; j <= pos.y + h / 2 + distanceBtwRooms && can; ++j)
                if (hasWallTile(new Vector3Int(i, j, 0))) can = false;
        }
        return can;
    }

    private void getCornerTiles()
    {
        int c = 0;
        for (int i = 0; i < tiles.Length; ++i)
        {
            if (tiles[i].name.Contains("Corner"))
                ++c;
        }
        cornerTiles = new Tile[c];
        for (int i = 0, a = 0; i < tiles.Length; ++i)
        {
            if (tiles[i].name.Contains("Corner"))
            {
                cornerTiles[a] = tiles[i].tile;
                ++a;
            }
        }
    }
    private Tile getCornerTile(int num)
    {
        return cornerTiles[num - 1];
    }

    private void getFloorTiles()
    {
        int c = 0;
        for (int i = 0; i < tiles.Length; ++i)
        {
            if (tiles[i].name.Contains("Floor"))
                ++c;
        }
        floorTiles = new Tile[c];
        for (int i = 0, a = 0; i < tiles.Length; ++i)
        {
            if (tiles[i].name.Contains("Floor"))
            {
                floorTiles[a] = tiles[i].tile;
                ++a;
            }
        }
    }
    private Tile getFloorTile()
    {        
        return floorTiles[Random.Range(0,floorTiles.Length)];
    }

    private void getWallTiles()
    {
        int c = 0;
        for (int i = 0; i < tiles.Length; ++i)
        {
            if (tiles[i].name.Contains("Wall"))
                ++c;
        }
        wallTiles = new Tile[c];
        for (int i = 0, a = 0; i < tiles.Length; ++i)
        {
            if (tiles[i].name.Contains("Wall"))
            {
                wallTiles[a] = tiles[i].tile;
                ++a;
            }
        }
    }

    private Tile getWallTile(int num)
    {        
        return wallTiles[num - 1];
    }

    private void getTurnTiles()
    {
        int c = 0;
        for (int i = 0; i < tiles.Length; ++i)
        {
            if (tiles[i].name.Contains("Turn"))
                ++c;
        }
        turnTiles = new Tile[c];
        for (int i = 0, a = 0; i < tiles.Length; ++i)
        {
            if (tiles[i].name.Contains("Turn"))
            {
                turnTiles[a] = tiles[i].tile;
                ++a;
            }
        }
    }
    private Tile getTurnTile(int num)
    {
        return turnTiles[num - 1];
    }

    private bool hasWallTile(Vector3Int pos)
    {
        return (tilemap.GetTile(pos) == null) ? false : (tilemap.GetTile(pos).name.Contains("Wall") || tilemap.GetTile(pos).name.Contains("Corner") || tilemap.GetTile(pos).name.Contains("Turn"));
    }

    private void addRoom(Room r)
    {
        int i = 0;
        while (rooms[i] != null)
            ++i;
        rooms[i] = r;
    }

    private int transformDirToWallTileLeft(int[] dir)
    {
        if (dir[0] == 3)
            return 1;
        else if (dir[0] == 2)
            return 2;
        else if (dir[0] == 1)
            return 3;
        else
            return 4;
    }

    private int transformDirToTurnTileLeftStart(int dir)
    {
        return dir + 1;
    }

    private int transformDirToTurnTileLeft(int[] dir)
    {
        if ((dir[0] == 1 && dir[1] == 0) || (dir[0] == 2 && dir[1] == 3))
        {
            return 1;
        }
        else if ((dir[0] == 2 && dir[1] == 1) || (dir[0] == 3 && dir[1] == 0))
        {
            return 2;
        }
        else if ((dir[0] == 3 && dir[1] == 2) || (dir[0] == 0 && dir[1] == 1))
        {
            return 3;
        }
        else
            return 4;
    }

    private int transformDirToCornerTileLeft(int[] dir)
    {
        if ((dir[0] == 3 && dir[1] == 0) || (dir[0] == 2 && dir[1] == 1))
        {
            return 1;
        }else if ((dir[0] == 1 && dir[1] == 0) || (dir[0] == 2 && dir[1] == 3))
        {
            return 2;
        }else if ((dir[0] == 1 && dir[1] == 2) || (dir[0] == 1 && dir[1] == 3))
        {
            return 3;
        }
        else
            return 4;
    }

    private void swapInts(ref int[] arr)
    {
        int aux = arr[0];
        arr[0] = arr[1];
        arr[1] = aux;
    }

}
