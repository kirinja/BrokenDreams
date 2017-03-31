using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {


    private Controller3D target;
    private Enemy02behaviour3D shooter;
    private Rigidbody rb;
	// Use this for initialization
	void Start () {

        rb = GetComponent<Rigidbody>();
        
	}

    public void Fire()
    {
        gameObject.SetActive(true);
        Vector3 shooterPos = shooter.transform.position;
        this.transform.position = shooterPos;
        rb.velocity = calculateVelocity(target.transform, 45f);
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

    /*public void Update()
    {
        Collider[] col = Physics.OverlapSphere(this.transform.position, 0.5f);
        int i = 0;
        while(i< col.Length)
        {
            if (col[i].CompareTag("Player"))
            {
                shooter.resetTime();
            }
        }
    }*/

    public void OnCollisionEnter(Collision col)
    {
        gameObject.SetActive(false);
        shooter.Fired = false;
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


