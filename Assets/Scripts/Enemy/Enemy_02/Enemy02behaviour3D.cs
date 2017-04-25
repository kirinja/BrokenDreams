using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy02behaviour3D : Enemy
{
    public int MaxHealth = 2;
    
    private int health;
    
    public float AttackCoolDown = 2f;
    
    private Controller3D target;

    private float timeSinceAttack;

    public float ArbitarySpeedMultiplier = 4.0f;

    private EnemyState state;
    
    public float HealthDropChance = 0.3f;
    public GameObject HealthDrop;
    private bool dead;

    public GameObject projectilePreFab;
    public Transform[] retreatPoints;
    [HideInInspector] private int rpIndex;
    [HideInInspector] private int rpThreshold;
    public float AggroRange = 10.0f;
    public LayerMask AggroMask;
    public LayerMask AggroCollisionMask;
    private AudioSource src;
    public AudioClip attackClip;
    public AudioClip aggroClip;
    public AudioClip deathClip;
    public AudioClip damageClip;
    private Transform platform;
    private Vector3 previousPlatformPosition;

    private LineRenderer _laserFocus;

    // Use this for initialization
    void Start()
    {
        health = MaxHealth;
        state = new Idle(this); //Base state for Enemy is idle, idle contains method for player detection
        //var p = Instantiate<GameObject>(projectilePreFab);
        //projectile = p.GetComponent<Projectile>();
        //projectile.setShooter(this);
        //projectile.setLifeTime(projLifeTime);
        //projectile.projSpeed = this.projSpeed;
        rpThreshold = retreatPoints.Length - 1;
        rpIndex = 0;
        src = GetComponent<AudioSource>();
        dead = false;
        Alive = true;

        _laserFocus = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            Aggro();
            if (target && timeSinceAttack >= AttackCoolDown && state.getCanShoot()) //Checks if you can shoot
            {
                resetTime();
                src.PlayOneShot(attackClip);
                Attack();
                //_laserFocus.enabled = false;
            }
            else
            {
                // here we do the line render stuff (tracking player)
                if (target)
                {
                    _laserFocus.enabled = true;
                    // if we have a target then do stuff
                    _laserFocus.SetPosition(0, transform.position);
                    _laserFocus.SetPosition(1, target.transform.position + new Vector3(0, target.transform.localScale.y / 2, 0));
                }
                else
                {
                    _laserFocus.enabled = false;
                }
                timeSinceAttack += Time.deltaTime;
            }
            state.Update();

            var platformObject = GetGround();
            if (platformObject)
            {
                if (platformObject.transform == platform)
                {
                    transform.position += platform.position - previousPlatformPosition;
                    //foreach (var point in retreatPoints)
                    //{
                    //    point.position += platform.position - previousPlatformPosition;
                    //}
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

    private GameObject GetGround()
    {
        // TODO add childing mechanic here (this should only be called in certain cituations and we want the movement on moving platforms to be consitent)
        RaycastHit hitInfo;
        //if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 1.5f))
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 1.5f, AggroCollisionMask))
        {
            return hitInfo.transform.gameObject;
        }
        return null;
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

    private void Aggro()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, AggroRange, AggroMask);
        int i = 0;
        RaycastHit hit;
        bool foundPlayer = false;
        for (int v = 0; v < col.Length; v++)
        {
            if (col[v].CompareTag("Player") && !Physics.Linecast(transform.position, col[i].transform.position, out hit, AggroCollisionMask))
            {
                setTarget(col[v].GetComponent<Controller3D>());
                getSource().PlayOneShot(aggroClip);
                foundPlayer = true;
                break;
            }
        }

        if (!foundPlayer)
        {
            setTarget(null);
            //changeState(new Idle(this)); //If not defeated spasm
        }
    }

    public void Attack()
    {
        var g = Object.Instantiate(projectilePreFab, transform.position, Quaternion.identity);
        g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, -1);
    }

    public override void Damage(int damage = 1) //Method to call when player hits an enemy
    {

        transform.Find("Damage").GetComponent<ParticleSystem>().Play();
        health -= damage;
        if (health <= 0)
        {
            Defeat();
        }
        else
        {
            src.PlayOneShot(damageClip);
            changeState(new Deal(this)); //If not defeated spasm
        }
    }

    private void Defeat()
    {
        StartCoroutine("deathTime");

        if (Drop == null)
        {
            if (UnityEngine.Random.value < HealthDropChance)
            {
                Drop = HealthDrop;
            }
        }
        dead = true;
        Alive = false;

        var g = Drop;
        if (g != null)
            GameObject.Instantiate(g, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
        src.PlayOneShot(deathClip);
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        _laserFocus.enabled = false;
    }

    public void resetTime() //Timer to shoot again starts when projectile hits something
    {
        
        timeSinceAttack = 0;
    }

    /*public void startCoolDown()
    {
        Fired = !Fired;
    }*/

    public override void changeState(EnemyState state) //Called by state when a transition is in order
    {
        this.state.Exit();
        this.state = state;
        state.Enter();
    }
    public void setTarget(Controller3D target) //Called by Idle when an enemy is found
    {
        this.target = target;
        //projectile.setTarget(target);
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
        if (other.CompareTag("Movable Object") || other.CompareTag("Wall"))
        {
            NextTarget();
        }
    }

    public AudioSource getSource()
    {
        return src;
    }

    public void Move(Vector3 vec3)
    {
        transform.position += vec3 * Time.deltaTime;
    }

    public void NextTarget()
    {
        rpIndex++;
        rpIndex = rpIndex > rpThreshold ? 0 : rpIndex--; //Chooses next goal when current goal is reached.
    }

    public Vector3 TargetPosition()
    {
        return retreatPoints[rpIndex].position;
    }

    public override GameObject Drop { get; set; }
    public override bool Alive { get; set; }
}
