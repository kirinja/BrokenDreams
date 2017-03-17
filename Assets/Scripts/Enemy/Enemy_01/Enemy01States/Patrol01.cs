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
    }

    public void Collision()
    {}

    public Patrol01(Enemy01Behaviour enemy, Vector3 vec){

        this.enemy = enemy;
        this.vec = vec;


    }

    
}
