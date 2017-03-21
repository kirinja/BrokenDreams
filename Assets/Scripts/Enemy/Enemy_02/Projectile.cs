using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {


    public Controller3D target;
    public Enemy02behaviour3D shooter;
    public Rigidbody rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        
        
        
	}

    public void Fire()
    {
        Vector3 shooterPos = new Vector3(shooter.transform.position.x, shooter.transform.position.y, shooter.transform.position.z);
        this.transform.position = shooterPos;
        rb.velocity = calculateVelocity(target.transform, 45f);
        Debug.Log("Åker nu");
    }

    public Vector3 calculateVelocity(Transform target, float angle)
    {
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
        shooter.Fired = false;
    }

    public void setTarget(Controller3D target)
    {
        this.target = target;
    }

}


