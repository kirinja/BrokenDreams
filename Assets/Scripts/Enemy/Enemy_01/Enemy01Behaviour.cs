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
    public LayerMask CollisionMask;
    
    private Enemy01State state;
    private int health;

    public override GameObject Drop
    {
        get; set;
    }

    // Use this for initialization
    void Start () {
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

        var g = Drop;
        if (g != null)
            GameObject.Instantiate(g, transform.position + new Vector3(0, 1.0f, 0), Quaternion.identity);

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
    }

    public override void changeState(EnemyState e)
    {
        throw new NotImplementedException();
    }
}
