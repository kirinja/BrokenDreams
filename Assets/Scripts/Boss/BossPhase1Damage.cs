using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhase1Damage : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Movable Object"))
        {
            // if the movable object gets into here then do damage on the boss
            // add a delay, need to wait until the object has landed at least
            StartCoroutine(Delay(1, other.gameObject));
            //Destroy(other.transform);
            //GetComponentInParent<BossBehaviour>().BossState.TakeDamage(1);
        }
    }

    IEnumerator Delay(float seconds, GameObject other)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(other);
        GetComponentInParent<BossBehaviour>().BossState.TakeDamage(1);
    }


}
