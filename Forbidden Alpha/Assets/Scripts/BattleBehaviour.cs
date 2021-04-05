using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Clase con la funcionalidad del personaje
/// </summary>
public class BattleBehaviour : MonoBehaviour
{
    //Variables para manejar movimineto en combate
    public int moveRange;
    private BoundsInt moveArea;

    // Estados del combate
    [HideInInspector]
    public StateManager combatState;

    // Variables de stats personajes máximas
    public int megas, ram, attack, defense, iniciative;

    // Stats de personajes en el momento
    [HideInInspector]
    public int currentMegas, currentRam, currentXp;

    // Detectar si es enemigo en combaye, marcar true en el prefab
    public bool isEnemy;

    // Start is called before the first frame update
    void Start()
    {
        //se inician las estadisticas del personaje
        combatState = StateManager.ooc;
        currentMegas = megas;
        currentRam = ram;
        currentXp = 0;
    }

    /// <summary>
    /// Getter de iniciativa de personaje
    /// </summary>
    public int getIniciative()
    {
        return iniciative;
    }

    /// <summary>
    /// Getter de area disponible del movimiento pasandole la posicion del objeto
    /// </summary>
    public BoundsInt getMoveArea(Vector3Int position)
    {
        moveArea.position = position + new Vector3Int(-1 * moveRange,-1 * moveRange, 0);
        moveArea.size = new Vector3Int(moveRange * 2 + 1, moveRange * 2 + 1, 1);
        return moveArea;
    }

    /// <summary>
    /// setter de estados de combate
    /// </summary>
    public void setState(int state)
    {
        switch (state)
        {
            case 0:
                combatState = StateManager.move;
                break;
            case 1:
                combatState = StateManager.attack;
                break;
            case 2:
                combatState = StateManager.ooc;
                break;
        }
    }

    /// <summary>
    /// Getter de estado de combate
    /// </summary>
    public StateManager getState()
    {
        return combatState;
    }

    /// <summary>
    /// Getter del texto de vida actual junto con el máximo
    /// </summary>
    public string getHpText()
    {
        return currentMegas + " / " + megas;
    }

    /// <summary>
    /// Getter del texto de ram actual junto con el máximo
    /// </summary>
    public string getRamText()
    {
        return currentRam + " / " + ram;
    }
}
