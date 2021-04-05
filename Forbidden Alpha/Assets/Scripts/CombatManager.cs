using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class CombatManager : MonoBehaviour
{
    /// <summary>
    /// Inicializacion de otros scripts y array de los personajes
    /// </summary>
    private BattleBehaviour currentChar;
    private MovementOOC moveController;
    public GameObject[] viruses;

    //Variables de combate
    #region CombatVariables
    public GameObject virusLeader;
    public GameObject[] characters;
    public Tilemap tilemap;
    public GameObject combatUi;
    public Camera camera;

    /// <summary>
    /// Ayuda visual para los comandos basicos
    /// </summary>
    private BoundsInt move;
    private BoundsInt attackArea;
    private BoundsInt skillRange;

    
    private bool inCombat;

    private int turnCounter = 0;
    private Color defaultTileColor;

    #endregion

    //Variables de UI
    #region UIVariables
    //Array de stats para la UI
    public StatsUI[] stats;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        moveController = virusLeader.GetComponent<MovementOOC>();
        defaultTileColor = tilemap.GetColor(moveController.getPosition());
        inCombat = false;
        setStatsUI();
        updateStats();
    }

    //El combate se establece manualmente ahora mismo, se debe actualizar a estar asociado con los triggers de sala
    void Update()
    {

        //la mecanica de click es solo para el ataque
        if (inCombat && currentChar.getState() == StateManager.move)
        {
            Vector3Int movePos = tilemap.WorldToCell(currentChar.transform.position);
            if (Input.GetKeyDown(KeyCode.W))
                movePos += new Vector3Int(0, 1, 0);
            else if (Input.GetKeyDown(KeyCode.D))
                movePos += new Vector3Int(1, 0, 0);
            else if (Input.GetKeyDown(KeyCode.S))
                movePos += new Vector3Int(0, -1, 0);
            else if (Input.GetKeyDown(KeyCode.A))
                movePos += new Vector3Int(-1, 0, 0);
            if(availableTile(movePos) && tilemap.HasTile(movePos))
                currentChar.transform.position = tilemap.GetCellCenterWorld(movePos);
            clearVisualHelp();
            startCombat();
        }
        
        //Codigo para la implementacion de la funcionalidad de ataque del combate
        
        /*if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            switch (currentChar.getState())
            {
                case StateManager.move:
                    Vector3Int clickPos = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (move.Contains(clickPos))
                    {
                        currentChar.transform.position = tilemap.GetCellCenterWorld(clickPos);
                        clearVisualHelp();
                        startCombat();
                    }
                    break;
                case StateManager.attack:
                    break;
            }
        }*/
        //adaptar todo en base a la insercion por la interfaz y activacion por el trigger de sala
        if (Input.GetKeyDown(KeyCode.Space) && !inCombat)//start combat
        {
            setCombat();  
        }
        else if (Input.GetKeyDown(KeyCode.Space) && inCombat) // end combat
        {
            endCombat();
            virusLeader.GetComponent<MovementOOC>().setPos(tilemap.WorldToCell(virusLeader.transform.position));
        }
    }

    //Aqui se maneja el combate
    #region Combat
    /// <summary>
    /// Metodo para establecer las propiedades de combate
    /// </summary>
    public void setCombat()
    {
        inCombat = true;
        virusLeader.GetComponent<MovementOOC>().setCanMove(false);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        int total = viruses.Length + enemies.Length;
        characters = new GameObject[total];
        int i;
        for (i = 0; i < viruses.Length; ++i)
            characters[i] = viruses[i];
        for (int j = 0; i < total; ++i, ++j)
            characters[i] = enemies[j];

        orderByInciative();
        TurnManager();
    }
    /// <summary>
    /// Terminar el combate
    /// </summary>
    public void endCombat()
    {
        combatUi.SetActive(false);
        inCombat = false;
        moveController.setCanMove(true);
        clearVisualHelp();
        camera.GetComponent<Follow>().setFollow(virusLeader);
    }

    /// <summary>
    /// Estados de combate 
    /// </summary>
    private void startCombat()
    {
        if(currentChar.isEnemy != true)
        {
            combatUi.SetActive(true);
            switch (currentChar.getState())
            {
                case StateManager.move:
                    movement();
                    break;
                case StateManager.attack:
                    attack();
                    break;
                case StateManager.ooc:
                    ++turnCounter;
                    TurnManager();
                    break;
            }
        }
        else
        {
            //aqui introducir comportamiento enemigo
            print("Soy un enemigo");
            ++turnCounter;
            TurnManager();
        }
        
    }

    /// <summary>
    /// Ordenar los personajes por iniciativa (Mayor a menor)
    /// </summary>
    public void orderByInciative()
    {
        GameObject virusAux;
        for (int i = 0; i < characters.Length; ++i)
        {
            for (int j = 0; j < characters.Length; ++j)
            {
                if (characters[i].GetComponent<BattleBehaviour>().getIniciative() > characters[j].GetComponent<BattleBehaviour>().getIniciative())
                {                    
                    virusAux = characters[i];
                    characters[i] = characters[j];
                    characters[j] = virusAux;
                }
            } 
        }
    }

    /// <summary>
    /// Funcion para manejar los turnos
    /// </summary>
    public void TurnManager()
    {
        if (turnCounter != characters.Length)
        {
            currentChar = characters[turnCounter].GetComponent<BattleBehaviour>();
        }
        else
            turnCounter = 0;
        if (currentChar.isEnemy != true)
            combatUi.SetActive(true);
        else
        {
            combatUi.SetActive(false);
            startCombat();
        }
        //camera follow for characters
        camera.GetComponent<Follow>().setFollow(characters[turnCounter]);
    }

    /// <summary>
    /// Ayuda visual para movimiento del jugador
    /// </summary>
    public void movement()
    {
        Vector3Int currentPosition = tilemap.WorldToCell(currentChar.transform.position);
        move = currentChar.getMoveArea(currentPosition);
        BoundsInt.PositionEnumerator tiles = move.allPositionsWithin;
        tiles = tiles.GetEnumerator();
        while (tiles.MoveNext())
        {
            if (availableTile(tiles.Current) && (tiles.Current.x - currentPosition.x == 0 || tiles.Current.y - currentPosition.y == 0))
            {
                tilemap.RemoveTileFlags(tiles.Current, TileFlags.LockColor);
                tilemap.SetColor(tiles.Current, new Color(0,0,1,0.8f));
            }
        }
    }

    /// <summary>
    /// Ayuda visual para el ataque
    /// </summary>
    public void attack()
    {
        Vector3Int currentPosition = tilemap.WorldToCell(currentChar.transform.position);
        attackArea = currentChar.getMoveArea(currentPosition);
        BoundsInt.PositionEnumerator tiles = attackArea.allPositionsWithin;
        tiles = tiles.GetEnumerator();
        while (tiles.MoveNext())
        {
            if (!currentPosition.Equals(tiles.Current))
            {
                tilemap.RemoveTileFlags(tiles.Current, TileFlags.LockColor);
                tilemap.SetColor(tiles.Current, new Color(1, 0, 0, 0.8f));
            }
        }
    }

    /// <summary>
    /// Metodo para eliminar toda ayuda visual
    /// </summary>
    public void clearVisualHelp()
    {
        BoundsInt.PositionEnumerator tiles = new BoundsInt.PositionEnumerator();
        switch (currentChar.getState())
        {
            case StateManager.move:
                tiles = move.allPositionsWithin;
                break;
            case StateManager.attack:
                tiles = attackArea.allPositionsWithin;
                break;
        }
        tiles = tiles.GetEnumerator();

        while (tiles.MoveNext())
        {
            tilemap.SetColor(tiles.Current,defaultTileColor);
        }
    }

    /// <summary>
    /// Metodo para comprobar si la casilla esta disponible
    /// </summary>
    /// <param name="p1"></param>
    /// <returns></returns>
    private bool availableTile(Vector3Int p1)
    {
        bool available = true;
        for(int i = 0;i < characters.Length && available; ++i)
        {
            if (p1.Equals(tilemap.WorldToCell(characters[i].transform.position)))
                available = false;
        }
        return available;
    }

    /// <summary>
    /// Setter de estado de combate
    /// </summary>
    /// <param name="state"></param>
    public void setState(int state)
    {
        currentChar.setState(state);
        startCombat();
    }

    #endregion
    //Aqui se maneja la UI
    #region UI

    /// <summary>
    /// Actualizar stats en UI
    /// </summary>
    private void updateStats()
    {
        for(int i = 0; i < stats.Length; ++i)
        {
            stats[i].update();
        }
    }

    /// <summary>
    /// Poner las stats en la UI
    /// </summary>
    private void setStatsUI()
    {
        for(int i = 0; i < stats.Length; ++i)
        {
            stats[i].character = viruses[i].GetComponent<BattleBehaviour>();
        }
    }

    #endregion

}
