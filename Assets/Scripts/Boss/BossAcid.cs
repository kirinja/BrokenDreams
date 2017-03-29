using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAcid : MonoBehaviour
{
    public float FallingSpeed = 10.0f;
    private bool falling = true;
    public float GroundTime = 5.0f;
    private float groundTimer;
    private Rigidbody rbody;
	// Use this for initialization
	void Start ()
	{
	    groundTimer = GroundTime;
	    rbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (falling)
        {
            transform.position -= new Vector3(0, Time.deltaTime * FallingSpeed, 0);
            rbody.position = transform.position;
        }
        else
        {
            if (groundTimer <= 0.0f)
                Destroy(gameObject);
            groundTimer -= Time.deltaTime;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        //if (!other.CompareTag("Player") || !other.CompareTag("Enemy"))
        //{
        //    transform.position -= new Vector3(0, other.transform.localScale.y, 0);
        //    rbody.position = transform.position;
        //    falling = false;
        //}
        
        // HACK this is hacky but seems to work?
        if (other.CompareTag("Player"))
        {
        }
        else if (other.CompareTag("Enemy"))
        {
        }
        else if (falling)
        {
            transform.position -= new Vector3(0, other.transform.localScale.y, 0);
            rbody.position = transform.position;
            falling = false;
        }
    }
}
