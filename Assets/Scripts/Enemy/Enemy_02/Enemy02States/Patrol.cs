using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : EnemyState {

    private Enemy02behaviour3D enemy;
    private Vector3 destination;

    public Patrol(Enemy02behaviour3D enemy)
    {
        this.enemy = enemy;
    }

    private void calculateRetreatPoint() //Enemy02behaviour3D contains an array for retreat points
    {

        // ignore z (&& enemy.transform.position.z == enemy.retreatPoints[enemy.rpIndex].transform.position.z)
        if (Math.Abs(enemy.transform.position.x - enemy.retreatPoints[enemy.rpIndex].transform.position.x) < 1.5f)
        {
            enemy.rpIndex++;
            enemy.rpIndex = enemy.rpIndex > enemy.rpThreshold ? 0 : enemy.rpIndex--; //Chooses next goal when current goal is reached.
        }

        //agent.destination = enemy.retreatPoints[enemy.rpIndex].position; //Sets destination
        destination = enemy.retreatPoints[enemy.rpIndex].position;
    }

    public void Update()
    {
        calculateRetreatPoint();
        
        // move towards the given point, only move towards X, ignore everything else
        // calculate which way to move
        var offX = destination.x - enemy.transform.position.x;
        enemy.Move(new Vector3(offX, 0, 0).normalized  * enemy.ArbitarySpeedMultiplier); // HACK
    }

    public void Enter()
    {
        //agent.Resume();
        //Start footstep loop?
    }

    public void Exit()
    {
        //agent.Stop();
        //Exit footstep loop?
    }

    public bool getCanShoot()
    {
        return true;
    }
    
}
