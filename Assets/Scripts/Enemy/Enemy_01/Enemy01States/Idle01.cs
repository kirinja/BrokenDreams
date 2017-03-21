using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Idle01 : Enemy01State {

    private Enemy01Behaviour enemy;

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
        int i = UnityEngine.Random.Range(1, 100) < 50 ? 1 : 0;
        changeState(i);
    }

    private void changeState(int i) //go into patrol
    {
        if (i == 0)
        {
            enemy.changeState(new Patrol01(enemy, enemy.vec));
        }
    }

    public Idle01(Enemy01Behaviour enemy)
    {
        this.enemy = enemy;
    }
    





}
