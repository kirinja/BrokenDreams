using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deal : EnemyState {

    private Enemy02behaviour3D enemy;
    private Vector3 orgPos;
    private float countdown = 1f;
    private float timer = 1.2f;
    private int count = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// UpdateTime is called once per frame
	public void Update () {
        countdown -= Time.deltaTime;
        if(countdown < timer - 0.1f)
        {
            spasm();
            count++;
        }
        if(count >= 5)
        {
            enemy.changeState(new Idle(enemy));
        }



	}

    public Deal(Enemy02behaviour3D enemy)
    {
        this.enemy = enemy;
        
        //orgPos = enemy.transform.position;
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
        Debug.Log("Entering deal");
        orgPos = enemy.transform.position;
        
        
        
    }

    private void spasm()
    {
        Debug.Log("Shake");
        enemy.transform.Translate(UnityEngine.Random.insideUnitSphere / 5);
    }

    public void Exit()
    {
        enemy.transform.position = orgPos;
        Debug.Log("Exiting deal");
    }

    

    public bool getCanShoot()
    {
        return false;
    }
}
