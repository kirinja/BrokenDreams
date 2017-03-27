using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Behaviour : Enemy
{
    public Vector3 StartVelocity;
    public float IdleTime;
    public float PatrolTime;
    public float GroundCheckDistance;

    //private Collider col;
    private Enemy01State state;
    private int health;

    public override GameObject Drop
    {
        get
        {
            throw new NotImplementedException();
        }

        set
        {
            throw new NotImplementedException();
        }
    }

    // Use this for initialization
    void Start () {
        //col = GetComponent<Collider>();
        //state = new Idle01(this, new Vector3(0,0,0));
        state = new Patrol01(this, StartVelocity);
        Debug.Log("Patrol");
	}
	
	// UpdateTime is called once per frame
	void Update () {
        state.Update();
	}

    public void OnCollisionEnter(Collision other)
    {
        state.Collision();
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Controller3D>().AttackPlayer(transform.position, 1);
        }
    }
    

    public void changeState(Enemy01State state)
    {
        this.state = state;
    }

    public Vector3 getVec()
    {
        return StartVelocity;
    }

    public void invertVec()
    {
        StartVelocity *= -1;
    }

    public override void Damage()
    {
        health--;
        if (health <= 0)
        {
            Defeat();
        }
    }

    private void Defeat()
    {
        StartCoroutine("deathTime");
        Destroy(this.gameObject);
    }

    private IEnumerator deathTime()
    {
        yield return new WaitForSeconds(0.3f);
    }

    public void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Controller3D>().AttackPlayer(transform.position, 1);
        }
        else if (!other.gameObject.CompareTag("Player Trigger"))
        {
            state.Collision();
        }

        /*if (other.CompareTag("Attack"))
        {
            Damage();
        }*/
    }

    public override void changeState(EnemyState e)
    {
        throw new NotImplementedException();
    }
}
