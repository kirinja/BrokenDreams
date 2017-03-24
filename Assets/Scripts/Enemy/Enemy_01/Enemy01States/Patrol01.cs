using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol01 : Enemy01State {

    private Enemy01Behaviour enemy;
    private float switchTimer;
    private float timetoSwitch;
    
    public void Enter()
    {}

    public void Exit()
    {}


    public void Update()
    {
        RaycastHit hitInfo;
        if (!(Physics.Raycast(enemy.transform.position, Vector3.down * enemy.GroundCheckDistance, out hitInfo) && hitInfo.transform.CompareTag("Wall")))
        {
            enemy.invertVec();
        }

        enemy.transform.localPosition += enemy.getVec() * Time.deltaTime;
        switchTimer += Time.deltaTime;
        if(switchTimer > enemy.PatrolTime)
        {
            changeState();
        }
    }

    public void Collision()
    {
        enemy.invertVec();
          
    }

    public Patrol01(Enemy01Behaviour enemy, Vector3 vec){

        this.enemy = enemy;
        switchTimer = 0f;
        
        

    }
    public void changeState()
    {
        {
            enemy.changeState(new Idle01(enemy));
            Debug.Log("Switching to Idle");
        }
    }

    
}
