﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Prime31.TransitionKit;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    //public GameObject[] Phase1Launch;
    //public GameObject[] Phase2Launch;

    public GameObject Phase1Launch;
    public GameObject Phase2Launch;
    //private int phaseIndex = 0;

    public GameObject PushableBox;

    public GameObject NavmeshTargets;

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
    
    public float MinStateSwitch = 7.5f;
    public float MaxStateSwitch = 15.0f;

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
    

    public AudioClip[] BossIdleSounds;
    public AudioClip[] BossProjSounds;
    public AudioClip[] BossSpawnSounds;
    public AudioClip[] BossDamageSounds;
    public AudioClip[] BossDeathSounds;

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
        //if (Input.GetKeyDown(KeyCode.P))
        //    if (BossState != null)
        //        BossState.TakeDamage(1);

	    if (HP <= 0 && BossState != null)
	    {
            BossState.Exit();
	        BossState = null;

            var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            gameManager.BeatLevel(SceneManager.GetActiveScene().name);
            gameManager.SaveToMemory();
            gameManager.SaveToFiles();
            

            var fishEye = new FishEyeTransition()
            {
                nextScene = "Hub",
                duration = 5.0f,
                size = 0.2f,
                zoom = 100.0f,
                colorSeparation = 0.1f
            };
            TransitionKit.instance.transitionWithDelegate(fishEye);
        }

	    if (BossState == null) return;

	    var newState = BossState.Execute();

        // change phase
	    if (newState == null) return;
        
        PlayBossDeathSound();
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

    public void PlayBossDeathSound()
    {

        int range = Random.Range(1, BossDeathSounds.Length);
        _bossDirectSounds.clip = BossDeathSounds[range];
        _bossDirectSounds.PlayOneShot(_bossDirectSounds.clip);
        BossDeathSounds[range] = BossDeathSounds[0];
        BossDeathSounds[0] = _bossDirectSounds.clip;
    }
}
