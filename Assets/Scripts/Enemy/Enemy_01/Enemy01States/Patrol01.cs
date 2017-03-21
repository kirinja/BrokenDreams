using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol01 : Enemy01State {

    private Enemy01Behaviour enemy;
    private Vector3 vec;

    public void Enter()
    {}

    public void Exit()
    {}


    public void Update()
    {
        enemy.transform.Translate(vec);
        //int i = UnityEngine.Random.Range(1, 100) < 50 ? 1 : 0;
        //changeState(i);
    }

    public void Collision()
    {
        vec *= -1;
    }

    public Patrol01(Enemy01Behaviour enemy, Vector3 vec){

        this.enemy = enemy;
        this.vec = vec;
        


    }
    public void changeState(int i)
    {
        if (i == 1)
        {
            enemy.changeState(new Idle01(enemy));
        }
    }

    
}
