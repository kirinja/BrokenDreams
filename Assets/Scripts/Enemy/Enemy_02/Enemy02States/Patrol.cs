﻿using System;
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
        if (Math.Abs(enemy.transform.position.x - enemy.TargetPosition().x) < 0.2f)
        {
            enemy.NextTarget();
        }

        //agent.destination = enemy.retreatPoints[enemy.rpIndex].position; //Sets destination
        destination = enemy.TargetPosition();
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
    {}

    public void Exit()
    {}

    public bool getCanShoot()
    {
        return true;
    }
    
}
