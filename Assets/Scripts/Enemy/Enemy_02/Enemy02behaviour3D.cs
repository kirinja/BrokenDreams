﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy02behaviour3D : MonoBehaviour
{

    private int health = 5;

    private float moveSpeed = 2f;

    private float attackCoolDown = 4f;

    private float projectileSpeed;

    public Controller3D target;

    private float timeSinceAttack = 4f;

    public Projectile projectile;

    public bool Fired;

    private EnemyState state;



    public Transform[] retreatPoints;
    public int rpIndex = 0;
    public int rpThreshold = 2;
    public Vector3 AggroRange;
    public LayerMask AggroMask;

    // Use this for initialization
    void Start()
    {

        state = new Idle(this); //Base state for Enemy is idle, idle contains method for player detection
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target && timeSinceAttack >= attackCoolDown && state.getCanShoot()) //Checks if you can shoot
        {
            resetTime();
            Fired = true;
            Attack();
        }
        if (!Fired)
        {
            timeSinceAttack += Time.deltaTime;
        }
        state.Update();
    }



    /*private void Aggro() {

        
        Collider[] col = Physics.OverlapSphere(this.transform.position, 1f);
        int i = 0;
        RaycastHit hit;
        while(i < col.Length && target ==null)
        {
            if (col[i].CompareTag("Player") && Physics.Linecast(this.transform.position, col[i].transform.position, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    target = col[i].GetComponent<Controller3D>();
                }
            }
        }
    }*/

    public void Attack() //Tells associated projectile to fire
    {
        Debug.Log("Skjuter");
        projectile.Fire();
        
        
    }

    public void Damage() //Method to call when player hits an enemy
    {
        health--;
        if(health <= 0)
        {
            Defeat();
        }
       // changeState(new Deal(this)); //If not defeated spasm
    }

    private void Defeat()
    {
        StartCoroutine("deathTime");
        Destroy(this);
    }

    public void resetTime() //Timer to shoot again starts when projectile hits something
    {
        
        timeSinceAttack = 0;
    }

    /*public void startCoolDown()
    {
        Fired = !Fired;
    }*/

    public void changeState(EnemyState state) //Called by state when a transition is in order
    {
        this.state = state;
    }
    public void setTarget(Controller3D target) //Called by Idle when an enemy is found
    {
        this.target = target;
        projectile.setTarget(target);
    }

    private IEnumerator deathTime()
    {
        yield return new WaitForSeconds(0.3f);
    }

    





}
