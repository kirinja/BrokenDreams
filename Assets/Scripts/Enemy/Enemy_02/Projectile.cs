using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {


    private Controller3D target;
    private Enemy02behaviour3D shooter;
    private Rigidbody rb;
    private float maxLifeTime;
    private float currentLifeTime;
    private AudioSource src;
    public AudioClip hitClip;
	// Use this for initialization
	void Start () {

        rb = GetComponent<Rigidbody>();
	    src = GetComponent<AudioSource>();

	}

    public void setLifeTime(float f)
    {
        maxLifeTime = f;
    }

    public void Fire()
    {
        Vector3 shooterPos = shooter.transform.position + Vector3.up * 0.25f;
        this.transform.position = shooterPos;
        rb.position = shooterPos;
        currentLifeTime = 0;
        rb.velocity = calculateVelocity(target.transform, 45f);
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
        //Need sound
    }

    public Vector3 calculateVelocity(Transform target, float angle)
    {
        if (!target) return Vector3.zero;
        var dir = target.position - transform.position;  // get target direction
        var h = dir.y;  // get height difference
        dir.y = 0;  // retain only the horizontal direction
        var dist = dir.magnitude;  // get horizontal distance
        var a = angle * Mathf.Deg2Rad;  // convert angle to radians
        dir.y = dist * Mathf.Tan(a);  // set dir to the elevation angle
        dist += h / Mathf.Tan(a);  // correct for small height differences
                                   // calculate the velocity magnitude
        var vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        // return the complete velocity vector
        return vel * dir.normalized; //Koden är orginellt skriven av unity user aldonaletto
    }

    public void Update()
    {
        /*Collider[] cola = Physics.OverlapSphere(this.transform.position, 0.5f);
        int i = 0;
        while(i< cola.Length)
        {
            if (cola[i].CompareTag("Player"))
            {
                shooter.resetTime();
            }*/

        if (shooter.Fired)
        {
            currentLifeTime += Time.deltaTime;
            if (currentLifeTime >= maxLifeTime)
            {
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<Collider>().enabled = false;
                shooter.Fired = false;
            }
        }
    }
    

    public void OnTriggerEnter(Collider cola)
    {
        if (cola.gameObject.CompareTag("Enemy") || cola.gameObject.CompareTag("Platform") || cola.isTrigger) return;
        src.PlayOneShot(hitClip);
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        shooter.Fired = false;

        if (cola.gameObject.CompareTag("Player"))
        {
            cola.gameObject.GetComponent<Controller3D>().AttackPlayer(transform.position, 1);
        }
        //Need sound
    }

    public void setTarget(Controller3D target)
    {
        rb = GetComponent<Rigidbody>();
        this.target = target;
    }

    public void setShooter(Enemy02behaviour3D enemy)
    {
        shooter = enemy;
    }

}


