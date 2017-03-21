using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : EnemyState {

    private Enemy02behaviour3D enemy;
    private NavMeshAgent agent;

    public Patrol(Enemy02behaviour3D enemy)
    {
        this.enemy = enemy;
        agent = enemy.GetComponent<NavMeshAgent>();
    }

    private void calculateRetreatPoint() //Enemy02behaviour3D contains an array for retreat points
    {


        if (enemy.transform.position.x == enemy.retreatPoints[enemy.rpIndex].transform.position.x
            && enemy.transform.position.z == enemy.retreatPoints[enemy.rpIndex].transform.position.z)
        {
            enemy.rpIndex++;
            enemy.rpIndex = enemy.rpIndex > enemy.rpThreshold ? 0 : enemy.rpIndex--; //Chooses next goal when current goal is reached.
        }

        agent.destination = enemy.retreatPoints[enemy.rpIndex].position; //Sets destination
    }

    public void Update()
    {
        calculateRetreatPoint();
    }

    public void Enter()
    { }

    public void Exit()
    { }

    public bool getCanShoot()
    {
        return true;
    }
    
}
