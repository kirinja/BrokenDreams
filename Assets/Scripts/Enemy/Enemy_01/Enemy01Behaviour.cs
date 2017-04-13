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
    public float HealthDropChance = 0.2f;
    public GameObject HealthDrop;

    
    public AudioClip deathClip;
    public AudioClip attackClip;
    public AudioClip damageClip;

    private Enemy01State state;
    private int health;
    private AudioSource src;
    private Transform platform;
    private Vector3 previousPlatformPosition;
    private bool dead;

    public override GameObject Drop
    {
        get; set;
    }

    // Use this for initialization
    void Start () {
        state = new Patrol01(this, StartVelocity);
        src = GetComponent<AudioSource>();
        dead = false;
	}
	
	// UpdateTime is called once per frame
	void Update () {
        if (!dead)
        {
            state.Update();
            var platformObject = GetGround();
            if (platformObject)
            {
                if (platformObject.transform == platform)
                {
                    transform.position += platform.position - previousPlatformPosition;
                    previousPlatformPosition = platform.position;
                }
                else
                {
                    platform = platformObject.transform;
                    previousPlatformPosition = platform.position;
                }
            }
        } else if(dead && !src.isPlaying)
        {
            Destroy(gameObject);
        }
        
    }

    public void OnCollisionEnter(Collision other)
    {
        state.Collision();
        if (other.gameObject.CompareTag("Player"))
        {
            src.PlayOneShot(attackClip);
            other.gameObject.GetComponent<Controller3D>().AttackPlayer(transform.position, 1);
            
        }
    }

    private GameObject GetGround()
    {
        RaycastHit hitInfo;
        //if (Physics.Raycast(transform.position, Vector3.down, out hitInfo))
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, GroundCheckDistance, CollisionMask))
        {
            return hitInfo.transform.gameObject;
        }
        return null;
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
        transform.Find("Damage").GetComponent<ParticleSystem>().Play();
        health--;
        if (health <= 0)
        {
            Defeat();
        }
        if (!dead)
        {
            src.PlayOneShot(damageClip);
        }
    }

    private void Defeat()
    {
        dead = true;
        StartCoroutine("deathTime");
        src.PlayOneShot(deathClip);

        if (Drop == null)
        {
            if (UnityEngine.Random.value < HealthDropChance)
            {
                Drop = HealthDrop;
            }
        }

        var g = Drop;
        if (g != null)
            GameObject.Instantiate(g, transform.position + new Vector3(0, 0.0f, 0), Quaternion.identity);
        src.PlayOneShot(deathClip);
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    private IEnumerator deathTime()
    {
        yield return new WaitForSeconds(0.3f);
    }

    public void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            src.PlayOneShot(attackClip);
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
