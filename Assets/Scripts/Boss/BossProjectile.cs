using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{

    private Vector3 _target;
    public float ProjectileSpeed = 5.0f;
    private Rigidbody rbody;

	// Use this for initialization
	void Start ()
	{
	    _target = GameObject.FindGameObjectWithTag("Player").transform.position;
        // need to lock Z value to same plane
	    var direction = _target - transform.position;
	    _target = direction.normalized;
	    rbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    transform.position += _target * Time.deltaTime * ProjectileSpeed;
        rbody.position = transform.position;
	}

    void OnTriggerEnter(Collider other)
    {
        // TODO: implement collision in a correct manner (ignore platforms but not walls?)
        // here we should spawn the acid stuff (whenever it hits a surface we're gonna instanciate an acid prefab on there, matching normals)
        //if (!other.CompareTag("Enemy")) // || !other.CompareTag("Platform"))
            //Destroy(gameObject);
    }
}
