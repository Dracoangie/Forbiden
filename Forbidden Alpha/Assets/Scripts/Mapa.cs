using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// Clase con el funcionamiento asociado a la creacion del mapa jugable
/// </summary>
public class Mapa : MonoBehaviour
{
    //variables de inicio
    public int minSalas;
    public int maxSalas;
    public int roomWidthMin;
    public int roomWidthMax;
    public int roomHeightMin;
    public int roomHeightMax;
    private int MAXANCHO = 50;
    private int MAXALTO = 50;
    public GameObject triggerSala;

    //variables del jugador
    public Transform groupStart;
    public MovementOOC jugador;

    //variable de control para un objeto tile
    private struct TileObj
    {
        public string name;
        public Tile tile;
    }

    //almacenamiento de tiles
    private DirectoryInfo tileDir;
    private Tile[] wallTiles;
    private Tile[] floorTiles;
    private Tile[] cornerTiles;
    private Tile[] turnTiles;
    private TileObj[] tiles;

    //variables para el control del mapa
    public int distanceBtwRooms;
    private int countNumRooms;
    private int numRooms;
    private int[,] numMap;
    private Room[] rooms;
    public Tilemap tilemap;
    // Start is called before the first frame update
    public void createMap()
    {
        //obtener las tiles de las carpetas
        if (tiles == null)
        {
            getTiles();
        }

        //almacenar las tiles en el controlador
        getWallTiles();
        getTurnTiles();
        getFloorTiles();
        getCornerTiles();

        //crear mapa de juego
        generateMapSize();
        rooms = new Room[countNumRooms];
        generateMap();
        generatePaths();
        generateTriggers();
        jugador.setMapa(numMap);
        startRoom();
    }

    /// <summary>
    /// Metodo para colocar a los personajes en una sala
    /// </summary>
    private void startRoom()
    {
        int room = Random.Range(0, countNumRooms + 1);
        Vector3Int roomCenter = rooms[room].getRoomCenter();
        jugador.setPos(roomCenter);
        groupStart.position = new Vector3(roomCenter.x + 0.37f, (roomCenter.y +2.25f));
    }
    /// <summary>
    /// Metodo para el control de la generacion del mapa
    /// </summary>
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
                if (countNumRooms > 0 && makeRoom == 1 && canRoom(currentCell, width, height))
                {
                    createRoom(currentCell, width, height);
                    --countNumRooms;
                }
            }
        }
        
    }

    /// <summary>
    /// Metodo para generar el tamaño del mapa jugable
    /// </summary>
    private void generateMapSize()
    {
        countNumRooms = Random.Range(minSalas,maxSalas+1);
        numRooms = countNumRooms;
        rooms = new Room[countNumRooms];
        numMap = new int[MAXANCHO, MAXALTO];
    }

    /// <summary>
    /// Metodo que obtiene todos los archivos de tipo asset de la carpeta Resources(obtencion de las tiles)
    /// </summary>
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
            if (f.Extension.Contains("asset"))
            {
                tiles[z].name = f.Name;
                tiles[z].tile = Resources.Load(Path.GetFileNameWithoutExtension(f.Name)) as Tile;
                ++z;
            }
        }
    }

    /// <summary>
    /// Metodo para pintar una sala en el tilemap, como las salas pueden ser de tamaños tanto pares como impares, todo debe ser de 2 tipos
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    private void createRoom(Vector3Int pos, int w, int h)
    {
        int distanceX = 0, distanceY = 0;
        if (w % 2 == 0)
            distanceX = 1;
        if (h % 2 == 0)
            distanceY = 1;
        Tile wall;
        Vector3Int position;
        int tileType;
        for (int j = pos.y - h / 2 - distanceY; j <= pos.y + h / 2; ++j)
        {
            for (int i = pos.x - w / 2 - distanceX; i <= pos.x + w / 2; ++i)
            {
                position = new Vector3Int(i, j, 0);
                if (j == pos.y - h / 2 - distanceY || j == pos.y + h / 2 || i == pos.x - w / 2 - distanceX || i == pos.x + w / 2)
                {
                    //bot wall
                    if (i < pos.x + w / 2 && j == pos.y - h / 2 - distanceY)
                        tileType = 1;
                    //right wall
                    else if (j < pos.y + h / 2 && i == pos.x + w / 2)
                        tileType = 2;
                    //top wall
                    else if (i > pos.x - w / 2 - distanceX && j == pos.y + h / 2)
                        tileType = 3;
                    //left wall
                    else
                        tileType = 4;
                    if ((j == pos.y + h / 2 || j == pos.y - h / 2 - distanceY) && (i == pos.x + w / 2 || i == pos.x - w / 2 - distanceX))
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

    /// <summary>
    /// Metodo para que todas las salas creen al menos un camino a otra sala
    /// </summary>
    private void generatePaths()
    {
        for (int i = 0; i < Room.numRoomsCreated;++i)
        {   
            makePath(ref rooms[i]);
            //rooms[i].paintSides(i);
        }
    }

    /// <summary>
    /// Metodo para crear un camino entre 2 salas
    /// </summary>
    /// <param name="r"></param>
    private void makePath(ref Room r)
    {
        int[] direction = new int[2];
        Room r2;
        direction = getRoom2Dir(out r2, ref r);
        Vector3Int position = r.getRoomPathWall(direction[0]);

        
        tilemap.SetTile(position, getFloorTile());
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

    /// <summary>
    /// metodo para generar los triggers de control de las salas
    /// </summary>
    private void generateTriggers()
    {

        for (int i = 0; i < Room.numRoomsCreated; ++i)
        {
            rooms[i].setTriggers(triggerSala, ref tilemap);
        }
        
    }
    /// <summary>
    /// Metodo para obtener la direccion a la que esta una sala de otra en base a la cercania y si tiene disponible el muro al que señala para la creacion de caminos
    /// </summary>
    /// <param name="rLink"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    private int[] getRoom2Dir(out Room rLink, ref Room r)
    {
        Room r2 = rooms[0];
        Vector2 direction = new Vector2(9999, 9999);
        Vector2 directionAux;
        rLink = new Room();
        int[] dir = new int[2] {0,1};
        //Busqueda de la sala mas cercana a la que poder hacer un camino
        for(int i = 1; i < Room.numRoomsCreated; ++i)
        {
            r2 = rooms[i];
            Vector3Int room1Center = r.getRoomCenter();
            Vector3Int room2Center = r2.getRoomCenter();
            directionAux = new Vector2(room2Center.x - room1Center.x, room2Center.y - room1Center.y);
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

    /// <summary>
    /// Metodo para crear trozos de camino en base a la direccion de la sala 1 a la sala 2
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="pos"></param>
    /// <param name="r"></param>
    /// <param name="r2"></param>
    /// <returns></returns>
    private Vector3Int makePathSection(ref int[] dir, Vector3Int pos, ref Room r, ref Room r2)
    {
        int[,] checks = new int[4,2] { { 0, 1 }, {1,0}, {0,-1}, {-1,0} };
        Vector3Int room2Center = r2.getRoomCenter();
        Vector2 posDir = new Vector2(room2Center.x - pos.x, room2Center.y - pos.y);
        Vector2 posAux = new Vector2();
        Vector3Int finalPos = new Vector3Int();
        Vector3Int position;
        for(int i = 0; i < checks.GetLength(0) && !r.getConnected(); ++i)
        {
            position = new Vector3Int(pos.x+checks[i,0],pos.y+checks[i,1],pos.z);
            if (!hasWallTile(position))
            {
                posAux.x = room2Center.x - position.x;
                posAux.y = room2Center.y - position.y;
                if (posAux.magnitude < posDir.magnitude)
                {
                    posDir = posAux;
                    finalPos = position;
                }
            }else if (r2.hasPosition(position))
            {
                finalPos = position;
                r.connect();
            }
        }

        //Codigo provisional para actualizar las tiles de camino
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

        //Debug para condicion extraña donde se creaba un camino a (0,0)
        if (finalPos.x != 0)
        {
            tilemap.SetTile(finalPos, getFloorTile());
            numMap[finalPos.x, finalPos.y] = 1;
        }
        return finalPos;

        //Mas codigo provisional para la actualización de la tiles en base a su direccion(Tiene bugs)
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

    /// <summary>
    /// Metodo para comprobar si en una posicion se puede crear una sala
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Metodo para obtener las tiles de tipo esquina para la creacion del mapa
    /// </summary>
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
    /// <summary>
    /// getter para obtener una tile de tipo esquina
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private Tile getCornerTile(int num)
    {
        return cornerTiles[num - 1];
    }

    /// <summary>
    /// Metodo para obtener las tiles de tipo suelo para la creacion del mapa
    /// </summary>
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
    /// <summary>
    /// getter para obtener una tile de tipo suelo
    /// </summary>
    /// <returns></returns>
    private Tile getFloorTile()
    {        
        return floorTiles[Random.Range(0,floorTiles.Length)];
    }


    /// <summary>
    /// Metodo para obtener las tiles de tipo muro para la creacion del mapa
    /// </summary>
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

    /// <summary>
    /// getter para obtener una tile de tipo muro
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private Tile getWallTile(int num)
    {        
        return wallTiles[num - 1];
    }

    /// <summary>
    /// Metodo para obtener las tiles de tipo giro para la creacion del mapa
    /// </summary>
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
    /// <summary>
    /// getter para obtener una tile de tipo giro
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private Tile getTurnTile(int num)
    {
        return turnTiles[num - 1];
    }
    /// <summary>
    /// Metodo para comprobar si una posicion es un muro
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool hasWallTile(Vector3Int pos)
    {
        return (tilemap.GetTile(pos) == null) ? false : (tilemap.GetTile(pos).name.Contains("Wall") || tilemap.GetTile(pos).name.Contains("Corner") || tilemap.GetTile(pos).name.Contains("Turn"));
    }
    /// <summary>
    /// Metodo para añadir la sala creada a la coleccion de salas
    /// </summary>
    /// <param name="r"></param>
    private void addRoom(Room r)
    {
        int i = 0;
        while (rooms[i] != null)
            ++i;
        rooms[i] = r;
    }

    /// <summary>
    /// Metodos para obtener la tile correspondiente a la direccion
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    #region Transformacion de direccion en tipo de tile
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
    #endregion

    /// <summary>
    /// Metodo para especificar la direccion predominante del vector direccion(entre 2 salas)
    /// </summary>
    /// <param name="arr"></param>
    private void swapInts(ref int[] arr)
    {
        int aux = arr[0];
        arr[0] = arr[1];
        arr[1] = aux;
    }

}
