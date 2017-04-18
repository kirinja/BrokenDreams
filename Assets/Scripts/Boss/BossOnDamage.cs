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
        // need to add a check so we cant do damage every frame, basically invincible timer
        var bossData = GetComponentInParent<BossBehaviour>();
        if (bossData.Invincible) return;


        bossData.Invincible = true;
        bossData.BossState.TakeDamage(1);
        // TODO maybe play particle system here. Definitly need to play particle system anyway

        //GetComponentInParent<BossBehaviour>().BossState.TakeDamage(1);
    }
}
