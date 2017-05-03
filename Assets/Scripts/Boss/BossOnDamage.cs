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
    
    public override void Damage(int damage = 1)
    {
        //throw new System.NotImplementedException();
        // need to add a check so we cant do damage every frame, basically invincible timer
        var bossData = GetComponentInParent<BossBehaviour>();
        if (bossData.Invincible) return;


        bossData.Invincible = true;
        bossData.BossState.TakeDamage(damage);

        //transform.Find("Damage").GetComponent<ParticleSystem>().Play();
        if (bossData.BossState.ToString().Equals("BossPhaseTwo")) // HACK
            transform.parent.Find("Damage").GetComponent<ParticleSystem>().Play(); // HACK
        else
        {
            transform.Find("Damage").GetComponent<ParticleSystem>().Play(); // HACK
        }

        //GetComponentInParent<BossBehaviour>().BossState.TakeDamage(1);
    }
}
