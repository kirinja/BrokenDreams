using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Prime31.TransitionKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

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
    public AudioClip BossDeathSound;
    public AudioClip Phase2Defend;
    public AudioClip Phase2Attack;

    [HideInInspector] public bool Invincible;
    private float _invincibleTimer;
    public float InvincibleTime = 1.0f;
    private bool _visible;
    private bool _shouldZoom;

    private Timer _playerPoseTimer;

    public GameObject CreditsPrefab;

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

	    if (BossDefeated())
	    {
	    }
	    if (BossState == null)
	    {
	        if (_shouldZoom)
	        {
	            //// lerp the camera
	            var camera2D = Camera.main;
	            //camera2D.Offset = new Vector3(0, 0, -10.0f);
	            //camera2D.transform.position = Vector3.Lerp(camera2D.transform.position, new Vector3(camera2D.transform.position.x, camera2D.transform.position.y, -10.0f), 0.05f);
	            var player = GameObject.FindGameObjectWithTag("Player");
	            camera2D.transform.position = Vector3.Lerp(camera2D.transform.position,
	                new Vector3(player.transform.position.x + 1.0f, player.transform.position.y + 0.5f, -10.0f), 0.03f);

	            if (Math.Abs(camera2D.transform.position.z - (-10.0f)) < 0.5f)
	            {
                    BossPhase3.SetActive(false);
                }
	        }

            // Animation things
	        if (_playerPoseTimer.Update(Time.deltaTime))
	        {
	            var player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<Animator>().SetTrigger("NextPose");
                _playerPoseTimer.ResetToSurplus();
            }
	    }
        else
            NextState();
	}

    private void BossInvincible()
    {
        //MeshRenderer[] renders;
        if (Invincible)
        {
            _invincibleTimer += Time.deltaTime;

            // add flashing possibly
            if (InvincibleTime % 0.2f < 0.1f)
            {
                // doesnt flash, have to look over
                //_visible = !_visible;
                //renders = GetComponentsInChildren<MeshRenderer>();
                //foreach (var r in renders)
                //    r.enabled = _visible;
            }

        }
        if (!(_invincibleTimer >= InvincibleTime)) return;
        
        Invincible = false;
        _invincibleTimer = 0.0f;

        //renders = GetComponentsInChildren<MeshRenderer>();
        //foreach (var r in renders)
        //    r.enabled = true;
        
    }

    private void NextState()
    {
        var newState = BossState.Execute();
        // change phase
        if (newState == null) return;

        PlayDeathSound();

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
        // TODO add some kind of cutscene or something, play sound etc
        if (HP > 0 || BossState == null) return false;

        BossState.Exit();
        BossState = null;

        StartCoroutine("ShouldZoomIn");
        StartCoroutine("BossDeafeatedCutscene");

        return true;
    }

    private IEnumerator ShouldZoomIn()
    {
        BossPhase3.GetComponent<SplineInterpolator>().enabled = false;

        var explosion = BossPhase3.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody r in explosion)
            r.isKinematic = false;

        yield return new WaitForSeconds(0.5f);
        _shouldZoom = true;
    }

    private IEnumerator BossDeafeatedCutscene()
    {
        // disable player inputs
        var player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Controller3D>().enabled = false;
        // make sure player is visible
        var renders = player.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renders)
            r.enabled = true;
        var hud = GameObject.Find("HUDCanvas");
        hud.SetActive(false);

        var camera2D = Camera.main.GetComponent<Camera2D>();
        camera2D.Offset = new Vector3(0, 0, 0);
        camera2D.enabled = false;

        // rotate player towards camera
        player.transform.LookAt(Camera.main.transform);

        // disable all audiosource
        //var allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        //foreach (AudioSource audioS in allAudioSources)
        //{
        //    audioS.Stop();
        //}

        // start music
        // instantiate a canvas object that shows credits and plays music
        // have waitforseconds be equal to the song time
        // mute every other sound in the scene
        var go = Instantiate(CreditsPrefab);
        var time = go.GetComponent<MusicPlayer>().MusicClip.length;

        player.GetComponent<Animator>().SetTrigger("StartPose");
        var poseTime = 5f; // How long each pose will take
        _playerPoseTimer = new Timer(poseTime);

        yield return new WaitForSeconds(time);


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
    }

    public void PlayIdleSound()
    {

        int range = Random.Range(1, BossIdleSounds.Length);
        _bossDirectSounds.clip = BossIdleSounds[range];
        _bossDirectSounds.PlayOneShot(_bossDirectSounds.clip);
        BossIdleSounds[range] = BossIdleSounds[0];
        BossIdleSounds[0] = _bossDirectSounds.clip;
    }

    public void PlayProjSound()
    {

        int range = Random.Range(1, BossProjSounds.Length);
        _bossProjSounds.clip = BossProjSounds[range];
        _bossProjSounds.PlayOneShot(_bossProjSounds.clip);
        BossProjSounds[range] = BossProjSounds[0];
        BossProjSounds[0] = _bossProjSounds.clip;
    }

    public void PlaySpawnSound()
    {

        int range = Random.Range(1, BossSpawnSounds.Length);
        _bossDirectSounds.clip = BossSpawnSounds[range];
        _bossDirectSounds.PlayOneShot(_bossDirectSounds.clip);
        BossSpawnSounds[range] = BossSpawnSounds[0];
        BossSpawnSounds[0] = _bossDirectSounds.clip;
    }

    public void PlayDamageSound()
    {

        int range = Random.Range(1, BossDamageSounds.Length);
        _bossDirectSounds.clip = BossDamageSounds[range];
        _bossDirectSounds.PlayOneShot(_bossDirectSounds.clip);
        BossDamageSounds[range] = BossDamageSounds[0];
        BossDamageSounds[0] = _bossDirectSounds.clip;
    }

    public void PlayDeathSound()
    {

        /*int range = Random.Range(1, BossDeathSounds.Length);
        _bossDirectSounds.clip = BossDeathSounds[range];
        _bossDirectSounds.PlayOneShot(_bossDirectSounds.clip);
        BossDeathSounds[range] = BossDeathSounds[0];
        BossDeathSounds[0] = _bossDirectSounds.clip;*/
        _bossDirectSounds.PlayOneShot(BossDeathSound);
    }

    public void PlayPhaseTwoAttackSound()
    {
        _bossDirectSounds.PlayOneShot(Phase2Attack);
    }

    public void PlayPhaseTwoDefendSound()
    {
        _bossDirectSounds.PlayOneShot(Phase2Defend);
    }
}
