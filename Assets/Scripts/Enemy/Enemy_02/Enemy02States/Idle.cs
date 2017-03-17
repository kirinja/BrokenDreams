using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : EnemyState {

    private Enemy02behaviour3D enemy;
    private Vector3 AggroRange;

    public Idle(Enemy02behaviour3D enemy)
    {
        this.enemy = enemy;
        AggroRange = new Vector3 ( 0, 0, 0 );


    }

    private void Aggro() //Checks in a box around enemy if a player is nearby
    {
        Collider[] col = Physics.OverlapBox(enemy.transform.position, AggroRange);
        int i = 0;
        while (i < col.Length)
        {
            if (col[i].CompareTag("Player")){
                enemy.setTarget(col[i].GetComponent<PlayerController3D>());
                enemy.changeState(new Patrol(enemy));
            }
        }
    }

    public void Update()
    {
        Aggro();
    }

    public void Enter()
    {}

    public void Exit()
    {}

    public bool getCanShoot()
    {
        return false;
    }
}
