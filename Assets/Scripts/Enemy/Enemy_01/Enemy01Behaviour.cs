using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


public class Enemy01Behaviour : Enemy
{
    public AudioClip attackClip;
    public LayerMask CollisionMask;
    public AudioClip damageClip;
    private bool dead;

    public AudioClip deathClip;
    public float GroundCheckDistance;
    private int health;
    public GameObject HealthDrop;
    public float HealthDropChance = 0.2f;
    public float IdleTime;
    public float PatrolTime;
    private Transform platform;
    private Vector3 previousPlatformPosition;
    private AudioSource src;
    public Vector3 StartVelocity;

    private Enemy01State state;


    private Bounds skinnedBounds
    {
        get
        {
            var bounds = GetComponent<BoxCollider>().bounds;
            bounds.Expand(-0.03f);
            return bounds;
        }
    }


    public override GameObject Drop { get; set; }

    public override bool Alive { get; set; }


    public raycastOrigins getUpdatedRaycastOrigins()
    {
        var SkinnedBounds = skinnedBounds;
        var raycastOrigins = new raycastOrigins();
        raycastOrigins.bottomBack = (new Vector3(SkinnedBounds.min.x, SkinnedBounds.min.y, SkinnedBounds.min.z) +
                                     new Vector3(SkinnedBounds.min.x, SkinnedBounds.min.y, SkinnedBounds.max.z)) / 2f;
        raycastOrigins.bottomFront = (new Vector3(SkinnedBounds.max.x, SkinnedBounds.min.y, SkinnedBounds.min.z) +
                                      new Vector3(SkinnedBounds.max.x, SkinnedBounds.min.y, SkinnedBounds.max.z)) / 2f;
        return raycastOrigins;
    }


    // Use this for initialization
    private void Start()
    {
        state = new Patrol01(this, StartVelocity);
        src = GetComponent<AudioSource>();
        dead = false;
    }


    // UpdateAbility is called once per frame
    private void Update()
    {
        if (!dead)
        {
            state.Update();
            var platformObject = GetGround();
            if (platformObject)
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
        else if (dead && !src.isPlaying)
        {
            Destroy(gameObject);
        }
    }


    public void OnCollisionStay(Collision other)
    {
        state.Collision();
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<Controller3D>().AttackPlayer(transform.position, 1))
            {
                src.PlayOneShot(attackClip);
            }
        }
    }


    public bool getDirection()
    {
        if (StartVelocity.x > 0)
            return true;

        return false;
    }


    private GameObject GetGround()
    {
        RaycastHit hitInfo;
        //if (Physics.Raycast(transform.position, Vector3.down, out hitInfo))
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, GroundCheckDistance, CollisionMask))
            return hitInfo.transform.gameObject;

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


    public override void Damage(int damage = 1)
    {
        transform.Find("Damage").GetComponent<ParticleSystem>().Play();
        health -= damage;
        if (health <= 0)
            Defeat();
        if (!dead)
            src.PlayOneShot(damageClip);
    }


    private void Defeat()
    {
        dead = true;
        StartCoroutine("deathTime");
        src.PlayOneShot(deathClip);

        if (Drop == null)
            if (Random.value < HealthDropChance)
                Drop = HealthDrop;

        var g = Drop;
        if (g != null)
            Instantiate(g, transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity);

        GetComponent<Collider>().enabled = false;
        var sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer s in sprites)
            s.enabled = false;

        // disable movable object children only
        for (var i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).CompareTag("Movable Object"))
                transform.GetChild(i).gameObject.SetActive(false);
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


    public void setVec(Vector3 vec)
    {
        StartVelocity = vec;
    }
}


public struct raycastOrigins
{
    public Vector3 bottomBack;
    public Vector3 bottomFront;
}