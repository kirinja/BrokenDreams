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
    private MeshRenderer[] render;

	// UpdateTime is called once per frame
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
        render = enemy.GetComponents<MeshRenderer>();
    }

    /*private IEnumerator spasmTime()
    {
        for (float i = 1; i >= 0; i -= 0.2f)
        {
            spasm();
            yield return new WaitForSeconds(0.16f);

        }
        Exit();
        
    }*/

    public void Enter()
    {
        
        //Need sound?
        //orgPos = enemy.transform.position;
    }

    public void Exit()
    {
        //enemy.transform.position = orgPos;
    }

    /*private void spasm()
    {
        enemy.transform.Translate(UnityEngine.Random.insideUnitSphere / 5 * Time.deltaTime);
    }*/

    public bool getCanShoot()
    {
        return false;
    }
}
