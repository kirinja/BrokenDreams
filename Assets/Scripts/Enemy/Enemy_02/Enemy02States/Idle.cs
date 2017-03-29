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
    private void Aggro() {
        
        Collider[] col = Physics.OverlapSphere(enemy.transform.position, 20f, enemy.AggroMask);
        int i = 0;
        //RaycastHit hit;
        for (int v = 0; v< col.Length; v++)
        {
            if (col[v].CompareTag("Player")) //&& Physics.Linecast(enemy.transform.position, col[i].transform.position, out hit)
            {
                //if (hit.collider.gameObject.CompareTag("Player"))
                //{
                enemy.setTarget(col[v].GetComponent<Controller3D>());
                enemy.changeState(new Patrol(this.enemy));
                //Alert sound
                //}
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
