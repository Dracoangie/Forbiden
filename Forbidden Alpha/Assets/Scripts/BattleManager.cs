using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    private  BattleManager instance;

    public  BattleManager GetInstance()
    {
        return instance;
    }

    [SerializeField] private Transform pfCharacterBattle;

    

    private CharactersBattle virusCB;
    private CharactersBattle enemyCB;
    private CharactersBattle activeCharacter;

    private States state;

    private enum States {Player, Enemy }


    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        virusCB = SpawnCharacters(true);
        enemyCB = SpawnCharacters(false);

        SetActiveCharacter(virusCB);
        state = States.Player;
    }

    private void Update()
    {
        if (state == States.Player)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                virusCB.Attack(enemyCB, () =>
                {
                    NextActiveCharacter();
                });

            }
        }
        
    }

    private CharactersBattle SpawnCharacters(bool isVirus)
    {
        Vector3 position;
        if (isVirus) position = new Vector3(-50, -5);
        else position = new Vector3(50, -5);

        Transform characterTransform = Instantiate(pfCharacterBattle, position, Quaternion.identity);
        CharactersBattle charactersBattle = characterTransform.GetComponent<CharactersBattle>();
        charactersBattle.Setup(isVirus);

        return charactersBattle;
    }
    
    private void SetActiveCharacter(CharactersBattle charactersBattle)
    {
        if (activeCharacter != null )
        {
            activeCharacter.HideActive();
        }
        activeCharacter = charactersBattle;
        activeCharacter.ShowActive();
    }

    private void NextActiveCharacter()
    {
        if (activeCharacter == virusCB)
        {
            SetActiveCharacter(enemyCB);
            state = States.Enemy;
            enemyCB.Attack(enemyCB, () =>
            {
                NextActiveCharacter();
            });
        }
        else
        {
            SetActiveCharacter(virusCB);
            state = States.Player;
        }
    }

}
