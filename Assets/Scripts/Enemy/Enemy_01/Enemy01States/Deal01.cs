using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deal01 : Enemy01State
{
    private Enemy01Behaviour enemy;
    private const float dealTime = 1f;
    private float activeTime;
    private bool visible;
    private MeshRenderer[] render;




    public void Collision()
    {
        throw new NotImplementedException();
    }

    public void Enter()
    {
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        activeTime += Time.deltaTime;
        if(activeTime % 0.2f > 0.1f)
        {
            foreach(var r in render)
            {
                r.enabled = visible;
            }
        }

        if(activeTime >= dealTime)
        {
            foreach(var r in render)
            {
                r.enabled = true;
            }

            enemy.changeState(new Idle01(enemy));
        }
    }

    public Deal01(Enemy01Behaviour enemy)
    {
        this.enemy = enemy;
        render = enemy.GetComponents<MeshRenderer>();
    }
}
