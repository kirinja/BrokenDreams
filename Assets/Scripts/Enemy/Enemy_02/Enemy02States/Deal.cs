using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Deal : EnemyState {

    private Enemy02behaviour3D enemy;
    private Vector3 orgPos;
    private float timeActive;
    private const float dealTime = 1f;
    private bool visible;
    private SpriteRenderer[] render;

	// UpdateAbility is called once per frame
	public void Update () {
        timeActive += Time.deltaTime;
        if(timeActive % 0.2f < 0.1f)
        {
            visible = !visible;
            foreach (var r in render)
            {
                r.enabled = visible;
            }
        }

	    if (timeActive >= dealTime)
	    {
            foreach (var r in render)
            {
                r.enabled = true;
            }
            enemy.changeState(new Idle(enemy));
        }
    }

    public Deal(Enemy02behaviour3D enemy)
    {
        this.enemy = enemy;
        render = enemy.GetComponentsInChildren<SpriteRenderer>();
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
