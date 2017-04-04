﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{

    private Vector3 _target;
    public float ProjectileSpeed = 5.0f;
    private Rigidbody _rbody;
    public float ProjectileLifeTime = 10.0f;

	// Use this for initialization
	void Start ()
	{
	    _target = GameObject.FindGameObjectWithTag("Player").transform.position;
        // need to lock Z value to same plane
	    var direction = _target - transform.position;
	    _target = direction.normalized;
	    _rbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (ProjectileLifeTime <= 0.0f)
            Destroy(gameObject);

	    transform.position += _target * Time.deltaTime * ProjectileSpeed;
        _rbody.position = transform.position;

	    ProjectileLifeTime -= Time.deltaTime;
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Destroy(gameObject);
    }
}
