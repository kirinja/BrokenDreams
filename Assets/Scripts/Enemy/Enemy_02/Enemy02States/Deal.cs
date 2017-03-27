using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deal : EnemyState {

    private Enemy02behaviour3D enemy;
    //private Vector3 orgPos;

	// Use this for initialization
	void Start () {
		
	}
	
	// UpdateTime is called once per frame
	public void Update () {
		
	}

    public Deal(Enemy02behaviour3D enemy)
    {
        this.enemy = enemy;
        //orgPos = enemy.transform.position;
    }

    private IEnumerator spasmTime()
    {
        for (float i = 1; i >= 0; i -= 0.2f)
        {
            spasm();

        }
        yield return new WaitForSeconds(0.16f);
    }

    public void Enter()
    {
        enemy.StartCoroutine("spasmTime");
        Exit();
    }

    private void spasm()
    {
        enemy.transform.position = UnityEngine.Random.insideUnitSphere + enemy.transform.position;
    }

    public void Exit()
    {
        enemy.changeState(new Patrol(enemy));
    }

    

    public bool getCanShoot()
    {
        return false;
    }
}
