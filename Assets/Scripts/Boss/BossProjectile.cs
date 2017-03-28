using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{

    private Vector3 _target;
    public float ProjectileSpeed = 5.0f;

	// Use this for initialization
	void Start ()
	{
	    _target = GameObject.FindGameObjectWithTag("Player").transform.position;
	    var direction = _target - transform.position;
	    _target = direction.normalized;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    transform.position += _target * Time.deltaTime * ProjectileSpeed;
        GetComponent<Rigidbody>().position = transform.position;
	}

    void OnTriggerEnter(Collider other)
    {
        // here we should spawn the acid stuff (whenever it hits a surface we're gonna instanciate an acid prefab on there, matching normals)
        if (!other.CompareTag("Enemy") || !other.CompareTag("Platform"))
            Destroy(gameObject);
    }
}
