using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Idle01 : Enemy01State {

    private Enemy01Behaviour enemy;
    private float switchTimer;
    private float timetoSwitch;

    public void Collision()
    {
        
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {
        
    }



    public void Update()
    {
            
        
        switchTimer += Time.deltaTime;
        if(switchTimer > enemy.idleTime)
        {
            changeState();
        }

    }

    private void changeState() //go into patrol
    {
        
         enemy.changeState(new Patrol01(enemy, enemy.vec));
         Debug.Log("Switching to patrol");
            
    }

    public Idle01(Enemy01Behaviour enemy)
    {
        this.enemy = enemy;
        switchTimer = 0f;
        
    }
    





}
