using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy02behaviour3D : Enemy
{
    private int health = 3;

    //private float moveSpeed = 2f;

    private float attackCoolDown = 4f;

    private float projectileSpeed;

    private Controller3D target;

    private float timeSinceAttack = 4f;

    private Projectile projectile;

    public bool Fired;

    private EnemyState state;

    public float projLifeTime;



    public GameObject projectilePreFab;
    public Transform[] retreatPoints;
    public int rpIndex;
    public int rpThreshold;
    public Vector3 AggroRange;
    public LayerMask AggroMask;
    private AudioSource src;
    public AudioClip attackClip;
    public AudioClip damageClip;
    public AudioClip aggroClip;
    public AudioClip deathClip;

    // Use this for initialization
    void Start()
    {
        state = new Idle(this); //Base state for Enemy is idle, idle contains method for player detection
        var p = Instantiate<GameObject>(projectilePreFab);
        projectile = p.GetComponent<Projectile>();
        projectile.setShooter(this);
        projectile.setLifeTime(projLifeTime);
        rpThreshold = retreatPoints.Length - 1;
        rpIndex = 0;
        src = GetComponent<AudioSource>();
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
        projectile.Fire();
    }

    public override void Damage() //Method to call when player hits an enemy
    {
        if(--health <= 0)
        {
            Defeat();
        }
        else
        {
            //changeState(new Deal(this)); //If not defeated spasm
        }
    }

    private void Defeat()
    {
        StartCoroutine("deathTime");
        Destroy(this.gameObject);
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
        state.Exit();
        this.state = state;
        state.Enter();
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

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Controller3D>().AttackPlayer(transform.position, 1);
        }
    }

    public AudioSource getSource()
    {
        return src;
    }

    public override GameObject Drop { get; set; }
}
