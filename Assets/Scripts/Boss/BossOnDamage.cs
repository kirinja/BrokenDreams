using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossOnDamage : Attackable {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public override void Damage()
    {
        //throw new System.NotImplementedException();
        GetComponentInParent<BossBehaviour>().BossState.TakeDamage(1);
    }
}
