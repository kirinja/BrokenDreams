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
        
        var origins = enemy.getUpdatedRaycastOrigins();
        if (!(Physics.Raycast(origins.bottomBack + enemy.getVec() * Time.deltaTime, Vector3.down, enemy.GroundCheckDistance, enemy.CollisionMask)) && enemy.getVec() == new Vector3(-3, 0, 0))
        {
            enemy.setVec(new Vector3(3,0,0));
            
        }
        if (!(Physics.Raycast(origins.bottomFront + enemy.getVec() * Time.deltaTime, Vector3.down, enemy.GroundCheckDistance, enemy.CollisionMask)) && enemy.getVec() == new Vector3(3,0,0))
        {
            enemy.setVec(new Vector3(-3, 0,0 ));
            
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
        }
    }

    
}
