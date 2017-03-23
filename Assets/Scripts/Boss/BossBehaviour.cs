using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    [HideInInspector] public int HP;
    private IBossPhaseState _bossState;

    public int HitsPhaseOne = 1;
    public int HitsPhaseTwo = 3;
    public int HitsPhaseThree = 5;

    //public float SwitchPhaseOne = 0.3f;
    //public float SwitchPhaseTwo = 0.3f;
    //public float SwitchPhaseThree = 0.3f;
    
    // enemy to spawn

    // how often we change state
    [Tooltip("This is hacky and should be removed. Right now every state switch is tied to this timer")]
    public float StateSwitchTimer = 5.0f;

    [HideInInspector] public float phase1;
    [HideInInspector] public float phase2;
    [HideInInspector] public float phase3;

	// Use this for initialization
	void Start () {
		_bossState = new BossPhaseOne();
        _bossState.Enter(this);
        /*
	    phase1 = HP - HP * SwitchPhaseOne;
	    phase2 = HP - HP * (SwitchPhaseOne + SwitchPhaseTwo);
	    phase3 = HP - HP;
        */

	    HP = HitsPhaseOne + HitsPhaseTwo + HitsPhaseThree;
	    phase1 = HP - HitsPhaseOne;
	    phase2 = phase1 - HitsPhaseTwo;
	    phase3 = phase2 - HitsPhaseThree;
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (Input.GetKeyDown(KeyCode.P))
            _bossState.TakeDamage(1);

	    if (HP <= 0)
	    {
	        Debug.Log("BOSS DEAD");
	        _bossState = null;
	    }

	    if (_bossState == null) return;

	    var newState = _bossState.Execute();

        // change phase
	    if (newState == null) return;
        
        Debug.Log("Changing phase - (Current Name: " + _bossState.GetType().Name + ")");
	    _bossState.Exit();
	    _bossState = newState;
	    newState.Enter(this);
	    // here we also need to control the boss arena?
	    // or maybe that should be seperate
	    // is this controlling only the boss behaviour or the boss arena as well?
	}

    void OnCollisionEnter(Collision other)
    {
        // here might be a problem, since we're reusing this event for every phase but the way we damage is different depending on state
        // might be able to hack it with an if state check, so in phase 1 only box or whatever can deal damage
        /*
        if (_bossState.GetType().Name.Equals("BossPhaseOne"))
        {
            Debug.Log("Damage in Phase 1");   
        }
        else if (_bossState.GetType().Name.Equals("BossPhaseTwo"))
        {
            Debug.Log("Damage in Phase 2");
        }
        else if (_bossState.GetType().Name.Equals("BossPhaseThree"))
        {
            Debug.Log("Damage in Phase 3");
        }*/

        // this might be hacky?
        switch (_bossState.GetType().Name)
        {
            case "BossPhaseOne":
                Debug.Log("Damage in Phase 1");
                break;
            case "BossPhaseTwo":
                Debug.Log("Damage in Phase 2");
                break;
            case "BossPhaseThree":
                Debug.Log("Damage in Phase 3");
                break;
        }
    }
}
