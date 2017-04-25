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
        if (other.CompareTag("Platform") || other.CompareTag("Wall"))
        {
            //GameObject.Instantiate(_bossData.Enemy1, _spawnPoints[v].transform.position + new Vector3(0, _spawnPoints[v].transform.localScale.y / 2 + _bossData.Enemy1.transform.localScale.y / 2, -1), Quaternion.identity);
            BoxCollider box = Enemy.transform.GetComponent<BoxCollider>();
            CapsuleCollider caps = Enemy.transform.GetComponent<CapsuleCollider>();
            var pos = new Vector3(other.gameObject.transform.position.x, other.transform.position.y, -1);
            var g = Instantiate(Enemy, 
                pos + new Vector3(0, other.transform.lossyScale.y / 2 + (box == null ? caps.height / 2 : box.size.y / 2), 0),
                Quaternion.identity);

            if (caps)
            {
                g.GetComponent<Enemy02behaviour3D>().retreatPoints = GetPlatformTargetPoints(other.gameObject); 
            }

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

    private Transform[] GetPlatformTargetPoints(GameObject gb)
    {
        // HACK
        var a = gb.transform.Find("TargetA");
        var b = gb.transform.Find("TargetB");
        Transform[] targets = new Transform[2];
        targets[0] = a;
        targets[1] = b;

        return targets;
    }
}
