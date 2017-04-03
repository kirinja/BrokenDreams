using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Deal : EnemyState {

    private Enemy02behaviour3D enemy;
    private Vector3 orgPos;
    private float timeActive;
    private const float dealTime = 1.2f;

	// Use this for initialization
	void Start () {
		
	}
	
	// UpdateTime is called once per frame
	public void Update () {
        spasm();
        timeActive += Time.deltaTime;
	    if (timeActive >= dealTime)
	    {
            enemy.changeState(new Idle(enemy));
        }
    }

    public Deal(Enemy02behaviour3D enemy)
    {
        this.enemy = enemy;
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
        enemy.getSource().PlayOneShot(enemy.aggroClip);
        orgPos = enemy.transform.position;
    }

    public void Exit()
    {
        enemy.transform.position = orgPos;
    }

    private void spasm()
    {
        enemy.transform.Translate(UnityEngine.Random.insideUnitSphere / 5 * Time.deltaTime);
    }

    public bool getCanShoot()
    {
        return false;
    }
}
