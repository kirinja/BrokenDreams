using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    [HideInInspector] public int HP;
    public IBossPhaseState BossState;

    public int HitsPhaseOne = 1;
    public int HitsPhaseTwo = 3;
    public int HitsPhaseThree = 3;

    //public float SwitchPhaseOne = 0.3f;
    //public float SwitchPhaseTwo = 0.3f;
    //public float SwitchPhaseThree = 0.3f;
    
    // enemy to spawn
    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject Projectiles;
    public GameObject Acid;

    public GameObject[] PhasePlatforms;
    //private int phaseIndex = 0;

    public GameObject PushableBox;

    [Tooltip("Amount of enemies boss should spawn during phase 1")]
    public int Phase1Spawn = 3;
    [Tooltip("Amount of enemies boss should spawn during phase 2")]
    public int Phase2Spawn = 2;
    [Tooltip("Amount of projectiles the boss should spawn during phase 2")]
    public int Phase2Projectiles = 1;
    [Tooltip("Amount of projectiles the boss should spawn during phase 3")]
    public int Phase3Projectiles = 3;

    [Tooltip("Interval between acid droppings")]
    public float AcidTimer;

    // how often we change state
    // HACK this should be randomized in an interval and different for each state/phase
    [Tooltip("This is hacky and should be removed. Right now every state switch is tied to this timer")]
    public float StateSwitchTimer = 5.0f;

    public Transform Phase2DefendPos;
    public Transform Phase2AttackPos;

    public GameObject BossPhase1;
    public GameObject BossPhase2;
    public GameObject BossPhase3;

    [HideInInspector] public float damageSwitchPhase1;
    [HideInInspector] public float damageSwitchPhase2;
    [HideInInspector] public float damageSwitchPhase3;

    private AudioSource[] _audioSources;
    private AudioSource _bossDirectSounds;
    private AudioSource _bossProjSounds;

    // TODO Boss defeated sounds? 
    public AudioClip[] BossIdleSounds;
    public AudioClip[] BossProjSounds;
    public AudioClip[] BossSpawnSounds;
    public AudioClip[] BossDamageSounds;

	// Use this for initialization
	void Start () {
		BossState = new BossPhaseOne();
        BossState.Enter(this);
        /*
	    damageSwitchPhase1 = HP - HP * SwitchPhaseOne;
	    damageSwitchPhase2 = HP - HP * (SwitchPhaseOne + SwitchPhaseTwo);
	    damageSwitchPhase3 = HP - HP;
        */

	    _audioSources = GetComponents<AudioSource>();
	    _bossDirectSounds = _audioSources[0];
	    _bossProjSounds = _audioSources[1];

	    HP = HitsPhaseOne + HitsPhaseTwo + HitsPhaseThree;
	    damageSwitchPhase1 = HP - HitsPhaseOne;
	    damageSwitchPhase2 = damageSwitchPhase1 - HitsPhaseTwo;
	    damageSwitchPhase3 = damageSwitchPhase2 - HitsPhaseThree;
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (Input.GetKeyDown(KeyCode.P))
            if (BossState != null)
                BossState.TakeDamage(1);

	    if (HP <= 0)
	    {
	        Debug.Log("BOSS DEAD");
	        BossState = null;
	    }

	    if (BossState == null) return;

	    var newState = BossState.Execute();

        // change phase
	    if (newState == null) return;
        
        Debug.Log("Changing phase - (Current Name: " + BossState.GetType().Name + ")");
        //PhasePlatforms[phaseIndex].SetActive(false);
	    BossState.Exit();
	    BossState = newState;
	    newState.Enter(this);
	    //++phaseIndex;
	    //if (phaseIndex >= PhasePlatforms.Length)
	    //    phaseIndex = PhasePlatforms.Length;

        //PhasePlatforms[phaseIndex].SetActive(true);
	    // here we also need to control the boss arena?
	    // or maybe that should be seperate
	    // is this controlling only the boss behaviour or the boss arena as well?
	}

    void OnCollisionEnter(Collision other)
    {
        // here might be a problem, since we're reusing this event for every phase but the way we damage is different depending on state
        // might be able to hack it with an if state check, so in phase 1 only box or whatever can deal damage
        /*
        if (BossState.GetType().Name.Equals("BossPhaseOne"))
        {
            Debug.Log("Damage in Phase 1");   
        }
        else if (BossState.GetType().Name.Equals("BossPhaseTwo"))
        {
            Debug.Log("Damage in Phase 2");
        }
        else if (BossState.GetType().Name.Equals("BossPhaseThree"))
        {
            Debug.Log("Damage in Phase 3");
        }*/

        // this might be hacky?
        switch (BossState.GetType().Name)
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
        /*
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Controller3D>().Damage();
        }*/
    }

    public void PlayBossIdleSound()
    {

        int range = Random.Range(1, BossIdleSounds.Length);
        _bossDirectSounds.clip = BossIdleSounds[range];
        _bossDirectSounds.PlayOneShot(_bossDirectSounds.clip);
        BossIdleSounds[range] = BossIdleSounds[0];
        BossIdleSounds[0] = _bossDirectSounds.clip;
    }

    public void PlayBossProjSound()
    {

        int range = Random.Range(1, BossProjSounds.Length);
        _bossProjSounds.clip = BossProjSounds[range];
        _bossProjSounds.PlayOneShot(_bossProjSounds.clip);
        BossProjSounds[range] = BossProjSounds[0];
        BossProjSounds[0] = _bossProjSounds.clip;
    }

    public void PlayBossSpawnSound()
    {

        int range = Random.Range(1, BossSpawnSounds.Length);
        _bossDirectSounds.clip = BossSpawnSounds[range];
        _bossDirectSounds.PlayOneShot(_bossDirectSounds.clip);
        BossSpawnSounds[range] = BossSpawnSounds[0];
        BossSpawnSounds[0] = _bossDirectSounds.clip;
    }

    public void PlayBossDamageSound()
    {

        int range = Random.Range(1, BossDamageSounds.Length);
        _bossDirectSounds.clip = BossDamageSounds[range];
        _bossDirectSounds.PlayOneShot(_bossDirectSounds.clip);
        BossDamageSounds[range] = BossDamageSounds[0];
        BossDamageSounds[0] = _bossDirectSounds.clip;
    }
}
