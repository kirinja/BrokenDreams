using System.Collections;
using UnityEngine;

public class Enemy02behaviour3D : Enemy
{
    

    public float AttackCoolDown = 2f;
    public float ArbitarySpeedMultiplier = 4.0f;
    public float HealthDropChance = 0.3f;
    public GameObject HealthDrop;
    public GameObject projectilePreFab;
    public GameObject ProjectileFirePosition;
    public Transform[] retreatPoints;
    public float AggroRange = 10.0f;
    public LayerMask AggroMask;
    public LayerMask LineOfSightMask;
    public AudioClip attackClip;
    public AudioClip deathClip;
    public AudioClip damageClip;
    public AudioClip _chargeClip;
    public AudioClip[] aggroClips;
    [HideInInspector] public bool Invincible;
    public int MaxHealth = 2;

    private int health;
    private Controller3D target;
    private float timeSinceAttack;
    private float _shootCooldown;
    private float _cooldownTimer;
    private float _internalAttackCd;
    private EnemyState state;
    private bool dead;
    private int rpIndex;
    private int rpThreshold;
    private AudioSource src;
    private AudioSource _source2;
    private bool _facingLeft = false;
    private float _projOrigX;
    private Transform platform;
    private Vector3 previousPlatformPosition;
    private LineRenderer _laserFocus;
    private float _invincibleTimer = 0.0f;
    private float InvincibleTime = 0.45f;
    private Rigidbody _rigidbody;
    private ParticleSystem _particleCharger;

    // Use this for initialization
    void Start()
    {
        health = MaxHealth;
        state = new Idle(this);
        rpThreshold = retreatPoints.Length - 1;
        rpIndex = 0;
        src = GetComponents<AudioSource>()[0];
        _source2 = GetComponents<AudioSource>()[1];
        _source2.clip = _chargeClip;

        dead = false;
        Alive = true;

        _laserFocus = GetComponent<LineRenderer>();

        // we're doing this to split the cooldown between "recharge" and "targeting"
        _shootCooldown = AttackCoolDown / 2;
        _internalAttackCd = AttackCoolDown / 2;

        _rigidbody = GetComponent<Rigidbody>();

        _projOrigX = ProjectileFirePosition.transform.localPosition.x;
        _particleCharger = ProjectileFirePosition.GetComponentInChildren<ParticleSystem>();
        _particleCharger.Stop();

        // child this object to the ground it's standing on (this so with moving platforms the object moves consitently on it)
        SetGround();
        Flip();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.Paused)
        {
            _source2.Pause();
            src.Pause();
        }
        if (!dead && !GameManager.Instance.Paused)
        {
            if (Invincible)
            {
                _invincibleTimer += Time.deltaTime;
            }
            if (_invincibleTimer >= InvincibleTime)
            {
                Invincible = false;
                _invincibleTimer = 0.0f;
            }
            Aggro();
            // if there is a target and the time since last attack is higher than the CD between attacks and we are in a state where we can shoot
            // then disable the laser sight, attack and reset the timers
            if (target && timeSinceAttack >= _internalAttackCd && state.getCanShoot()) //Checks if you can shoot
            {
                _particleCharger.Stop();
                _particleCharger.Clear();
                _source2.Stop();
                Attack();
                resetTime();
            }
            else
            {
                // if we cant fire then enable the laser focus and count up the cooldowns
                if (target && _cooldownTimer >= _shootCooldown)
                {
                    _particleCharger.Play();
                    _laserFocus.enabled = true;
                    // if we have a target then do stuff
                    _laserFocus.SetPosition(0, ProjectileFirePosition.transform.position);
                    _laserFocus.SetPosition(1, target.transform.position + new Vector3(0, target.transform.localScale.y / 2, 0));
                    // here we can probaly use a charge up sound
                    if (!_source2.isPlaying)
                    {
                        _source2.Play();
                    }
                    timeSinceAttack += Time.deltaTime;
                }
                else
                {
                    _particleCharger.Stop();
                    _particleCharger.Clear();
                    _laserFocus.enabled = false;
                    _source2.Stop();
                }
                
            }

            if (target)
            {
                _cooldownTimer += Time.deltaTime;
            }
            state.Update();
            _rigidbody.position = transform.position;
        } else if(dead && !src.isPlaying)
        {
            Destroy(gameObject);
        }
    }

    private GameObject GetGround()
    {
        // TODO add childing mechanic here (this should only be called in certain cituations and we want the movement on moving platforms to be consitent)
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 1.5f, LineOfSightMask))
        {
            return hitInfo.transform.gameObject;
        }
        return null;
    }

    private void SetGround()
    {
        // HACK this could break down, but should be enough for the current problems
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 1.5f, LineOfSightMask))
        {
            transform.SetParent(hitInfo.transform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, 0.10f);
        Gizmos.DrawSphere(ProjectileFirePosition.transform.position, AggroRange);
        
    }

    private void Aggro()
    {
        Collider[] col = Physics.OverlapSphere(ProjectileFirePosition.transform.position, AggroRange, AggroMask);
        int i = 0;
        RaycastHit hit;
        bool foundPlayer = false;
        for (int v = 0; v < col.Length; v++)
        {
            //Debug.DrawLine(ProjectileFirePosition.transform.position, col[i].transform.position, Color.green);
            if (col[v].CompareTag("Player") && 
                !Physics.Linecast(
                    ProjectileFirePosition.transform.position,
                col[i].transform.position + new Vector3(0, col[i].transform.localScale.y / 2, 0),
                out hit,
                LineOfSightMask)) // HACK
            {
                //Debug.DrawLine(ProjectileFirePosition.transform.position, col[i].transform.position + new Vector3(0, col[i].transform.localScale.y / 2, 0), Color.red);
                if (!src.isPlaying && target == null)
                {
                    int index = Random.Range(0, aggroClips.Length);
                    src.PlayOneShot(aggroClips[index]);
                }
                setTarget(col[v].GetComponent<Controller3D>());
                foundPlayer = true;
                break;
            }
        }

        if (!foundPlayer)
        {
            setTarget(null);
        }
    }

    public void Attack()
    {
        var g = Object.Instantiate(projectilePreFab, ProjectileFirePosition.transform.position, Quaternion.identity);
        g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, -1);
        src.pitch = 1;
        src.PlayOneShot(attackClip);
    }

    public override void Damage(int damage = 1) //Method to call when player hits an enemy
    {
        if (Invincible) return;

        Invincible = true;
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
        _particleCharger.Stop();
        _particleCharger.Clear();
        _source2.Stop();
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
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        _laserFocus.enabled = false;
    }

    public void resetTime() //Timer to shoot again starts when projectile hits something
    {
        
        timeSinceAttack = 0;
        _cooldownTimer = 0;
    }
    
    public override void changeState(EnemyState state) //Called by state when a transition is in order
    {
        this.state.Exit();
        this.state = state;
        state.Enter();
    }
    public void setTarget(Controller3D target) //Called by Idle when an enemy is found
    {
        this.target = target;
    }

    
    public void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.CompareTag("Player"))
        //{
        //    other.gameObject.GetComponent<Controller3D>().AttackPlayer(transform.position, 1);
        //}
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
        
        Flip();
    }

    private void Flip()
    {
        // check if the target is to the right of us
        // if the retreat point position is higher than our position then the retreat point is to the right of us
        if (TargetPosition().x >= transform.position.x)
            _facingLeft = false;
        else if (TargetPosition().x < transform.position.x)
            _facingLeft = true;

        GetComponentInChildren<SpriteRenderer>().flipX = _facingLeft;

        // this might break, since we just flip the fire position rather than check if it's left or right we're facing
        // this seems buggy
        ProjectileFirePosition.transform.localPosition =
            new Vector3(_facingLeft ? -_projOrigX : _projOrigX,
                ProjectileFirePosition.transform.localPosition.y, ProjectileFirePosition.transform.localPosition.z);
    }

    public Vector3 TargetPosition()
    {
        return retreatPoints[rpIndex].position;
    }

    public override GameObject Drop { get; set; }
    public override bool Alive { get; set; }
}
