using System.Collections;
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
    public int HitsPhaseTwo = 2;
    public int HitsPhaseThree = 3;
    
    
    // enemy to spawn
    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject Projectiles;
    public GameObject Acid;

    public GameObject[] PhasePlatforms;

    public GameObject Phase1Launch;
    public GameObject Phase2Launch;

    public GameObject PushableBox;
    public GameObject HealthPickUp;

    [Tooltip("Amount of enemies boss should spawn during phase 1")]
    public int Phase1Spawn = 3;
    [Tooltip("Amount of enemies boss should spawn during phase 2")]
    public int Phase2Spawn = 2;
    [Tooltip("Amount of projectiles the boss should spawn during phase 2")]
    public int Phase2Projectiles = 1;
    [Tooltip("Amount of projectiles the boss should spawn during phase 3")]
    public int Phase3Projectiles = 3;

    [Tooltip("Time between boss projectiles (in seconds)")]
    public float TimeBetweenShots = 0.5f;

    [Tooltip("Interval between acid droppings")]
    public float AcidTimer;

    [Tooltip("Max amount of enemies allowed on a platform")]
    public int MaxEnemiesPerPlatfor = 2;

    [Tooltip("Max amount of tries the boss try to spawn before it determins it can't")]
    public int MaxTrySpawnCycles = 10;

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

    [HideInInspector] public bool Invincible;
    [HideInInspector] private float _invincibleTimer = 0.0f;
    [HideInInspector] private float _invincibleTime = 1.0f;
    private bool _visible;

	// Use this for initialization
	void Start () {
		BossState = new BossPhaseOne();
        BossState.Enter(this);

        _audioSources = GetComponents<AudioSource>();
	    _bossDirectSounds = _audioSources[0];
	    _bossProjSounds = _audioSources[1];

	    HP = HitsPhaseOne + HitsPhaseTwo + HitsPhaseThree;
	    damageSwitchPhase1 = HP - HitsPhaseOne;
	    damageSwitchPhase2 = damageSwitchPhase1 - HitsPhaseTwo;
	    damageSwitchPhase3 = damageSwitchPhase2 - HitsPhaseThree;

	    _visible = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (Input.GetKeyDown(KeyCode.P))
            if (BossState != null)
                BossState.TakeDamage(1);
	    
        BossInvincible();

	    if (BossDefeated()) return;
	    if (BossState == null) return;

        NextState();
	}

    private void BossInvincible()
    {
        MeshRenderer[] renders;
        if (Invincible)
        {
            _invincibleTimer += Time.deltaTime;

            // add flashing possibly
            if (_invincibleTime % 0.2f < 0.1f)
            {
                // doesnt flash, have to look over
                _visible = !_visible;
                renders = GetComponentsInChildren<MeshRenderer>();
                foreach (var r in renders)
                    r.enabled = _visible;
            }

        }
        if (!(_invincibleTimer >= _invincibleTime)) return;
        
        Invincible = false;
        _invincibleTimer = 0.0f;

        renders = GetComponentsInChildren<MeshRenderer>();
        foreach (var r in renders)
            r.enabled = true;
        
    }

    private void NextState()
    {
        var newState = BossState.Execute();
        // change phase
        if (newState == null) return;

        PlayBossDeathSound();

        // heal player between boss phases
        //var player = GameObject.FindGameObjectWithTag("Player");
        //var hpToHeal = player.GetComponent<PlayerAttributes>().MaxHP - player.GetComponent<PlayerAttributes>().currentHealth;
        //player.GetComponent<PlayerHealth>().Heal(hpToHeal);

        // TODO Make this neater
        Object.Instantiate(HealthPickUp, new Vector3(-6.25f, 1.5f, -1f), Quaternion.identity);
        Object.Instantiate(HealthPickUp, new Vector3(-2.25f, 1.5f, -1f), Quaternion.identity);

        BossState.Exit();
        BossState = newState;
        newState.Enter(this);
    }

    private bool BossDefeated()
    {
        if (HP > 0 || BossState == null) return false;

        BossState.Exit();
        BossState = null;

        var gameManager = GameManager.Instance;
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
        return true;
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
