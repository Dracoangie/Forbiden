using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharactersBattle : MonoBehaviour
{
    //private Anim Anim;
    
    private enum States { Player, Anim, Enemy }
    private States state;
    private Vector3 runTarget;
    private Action runCompleted;
    private Action attackCompleted;


    private bool isVirus;

    private GameObject activeCharacter;

    private void Awake()
    {
        //Anim = GetComponent<Anim>();
        activeCharacter = transform.Find("Activo").gameObject;
        HideActive();
    }

    private void Start()
    {

    }

    private void Update()
    {
        switch (state)
        {
            case States.Player:
                break;
            case States.Anim:
                float runSpeed = 10f;
                transform.position += (runTarget - GetPosition()) * runSpeed * Time.deltaTime;
                float nearEnemy = 1f;
                if (Vector3.Distance(GetPosition(), runTarget) < nearEnemy)
                {
                    transform.position = runTarget;
                    runCompleted();
                }
                break;
            case States.Enemy:
                break;
        }
    }
    
    public void Setup(bool isVirus)
    {
        if (isVirus)
        {
           // Anim.SetAnim();
            //Texture = BattleManager.GetInstance().virusSprite;
        }
        else
        {
            //Anim.SetAnim();
            //Anim.GetMaterial().mainTexture = BattleManager.GetInstance().enemySprite;
        }
    }
    
    public void Attack(CharactersBattle runTarget, Action attackCompleted)
    {
        
        Vector3 runTargetPosition = (runTarget.GetPosition() - GetPosition()).normalized;
        Vector3 startingPosition = GetPosition();

        runToPosition(runTargetPosition, () => {
            
            Vector3 attackDir = (runTarget.GetPosition() - GetPosition()).normalized;
            runToPosition(startingPosition, () => {
                state = States.Player;
            });
            attackCompleted();
        });
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void runToPosition(Vector3 runTarget, Action runCompleted)
    {
        
        this.runTarget = runTarget;
        this.runCompleted = runCompleted;
        state = States.Anim;
    }

    public void HideActive()
    {
        activeCharacter.SetActive(false);
    }

    public void ShowActive()
    {
        activeCharacter.SetActive(true);
    }
}
