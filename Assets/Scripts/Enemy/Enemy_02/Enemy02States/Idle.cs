using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : EnemyState {

    private Enemy02behaviour3D enemy;
    

    public Idle(Enemy02behaviour3D enemy)
    {
        this.enemy = enemy;


    }

    /*private void Aggro() //Checks in a box around enemy if a player is nearby
    {
        Collider[] col = Physics.OverlapBox(enemy.transform.position, enemy.AggroRange);
        int i = 0;
        while (i < col.Length)
        {
            if (col[i].CompareTag("Player")){
                enemy.setTarget(col[i].GetComponent<Controller3D>());
                enemy.changeState(new Patrol(enemy));
            }
        }
    }*/
    private void Aggro()
    {
        Collider[] col = Physics.OverlapSphere(enemy.ProjectileFirePosition.transform.position, enemy.AggroRange, enemy.AggroMask);
        int i = 0;
        for (int v = 0; v < col.Length; v++)
        {
            RaycastHit hit;
            if (!col[v].CompareTag("Player") || Physics.Linecast(enemy.ProjectileFirePosition.transform.position,
                    col[i].transform.position + new Vector3(0, col[i].transform.localScale.y / 2, 0),
                    out hit,
                    enemy.LineOfSightMask)) continue;
            enemy.setTarget(col[v].GetComponent<Controller3D>());
            enemy.changeState(new Patrol(enemy));
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
