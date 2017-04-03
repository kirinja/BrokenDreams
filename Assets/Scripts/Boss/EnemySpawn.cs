using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [HideInInspector]
    public GameObject Enemy;
    private Rigidbody rbody;
    [HideInInspector]
    public GameObject PushableBox;

	// Use this for initialization
	void Start ()
	{
	    rbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    rbody.position = transform.position;
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Platform"))
        {
            //GameObject.Instantiate(_bossData.Enemy1, _spawnPoints[v].transform.position + new Vector3(0, _spawnPoints[v].transform.localScale.y / 2 + _bossData.Enemy1.transform.localScale.y / 2, -1), Quaternion.identity);
            BoxCollider box = Enemy.transform.GetComponent<BoxCollider>();
            CapsuleCollider caps = Enemy.transform.GetComponent<CapsuleCollider>();
            var g = Instantiate(Enemy, 
                other.gameObject.transform.position + new Vector3(0, other.transform.lossyScale.y / 2 + (box == null ? caps.height / 2 : box.size.y / 2), 0),
                Quaternion.identity);

            if (transform.childCount > 0)
            {
                var c = transform.GetChild(0);
                c.position = g.transform.position + new Vector3(0, 0.5f, 0);
                c.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                c.SetParent(g.transform);
                g.GetComponent<Enemy>().Drop = PushableBox;
            }

            
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
