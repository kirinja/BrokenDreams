using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deal : EnemyState {

    private Enemy02behaviour3D enemy;
    private Vector3 orgPos;

	// Use this for initialization
	void Start () {
		
	}
	
	// UpdateTime is called once per frame
	public void Update () {
		
	}

    public Deal(Enemy02behaviour3D enemy)
    {
        this.enemy = enemy;
        Enter();
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
        enemy.StartCoroutine("spasmTime");
        
    }

    /*private void spasm()
    {
        enemy.transform.translate(UnityEngine.Random.insideUnitSphere / 10);
    }*/

    public void Exit()
    {
        enemy.transform.position = orgPos;
        enemy.changeState(new Idle(enemy));
        Debug.Log("Exiting deal");
    }

    

    public bool getCanShoot()
    {
        return false;
    }
}
