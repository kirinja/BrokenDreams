using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossSubTwoDefend: IBossSubState
{

    private BossBehaviour _bossData;
    private GameObject _head;
    private float _timer;
    private bool _playing;

    private bool _spawned;
    private float _spawnTimer;
    private const float TimeBetweenSpawns = 1.0f;
    private int _spawnCounter;

    private int[] _platformIds;
    private GameObject[] _spawnPoints;

    private GameObject[] _arcs;

    private int _trySpawnCounter;

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        _timer = new System.Random().Next((int)_bossData.MinStateSwitch, (int)_bossData.MaxStateSwitch); // HACK

        _head = GameObject.Find("Head");
        _playing = false;

        _spawnCounter = 0;
        _trySpawnCounter = 0;
        _spawnTimer = TimeBetweenSpawns;

        _spawnPoints = GameObject.FindGameObjectsWithTag("Platform");
        _platformIds = new int[_spawnPoints.Length];
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            _platformIds[i] = _spawnPoints[i].GetComponent<PlatformID>().PlatformId;
        }

        foreach (var id in _platformIds)
        {
            CheckPlatform(id);
        }

        _arcs = new GameObject[_bossData.Phase2Launch.transform.childCount];
        for (int i = 0; i < _bossData.Phase2Launch.transform.childCount; i++)
        {
            _arcs[i] = _bossData.Phase2Launch.transform.GetChild(i).gameObject;
        }
    }

    public IBossSubState Execute()
    {
        HideHead();

        // here we need to add so we cant flood the scene
        Spawn();

        // give up on trying to spawn (possibly move out)
        

        if (CanPlayIdleSound())
            PlayIdleSound();

        UpdateTimers();

        if (!(_timer <= 0.0f)) return null;
        
        if (Random.value <= 0.5)
            return new BossSubTwoIdle();

        return new BossSubTwoAttack();
    }

    public void Exit()
    {
        _spawned = false;
        _spawnCounter = 0;
    }

    public bool Alive()
    {
        return _bossData.HP <= _bossData.damageSwitchPhase2;
    }

    public void TakeDamage(int value)
    {
        _bossData.HP -= value;
    }

    private void HideHead()
    {
        _head.transform.position = _bossData.Phase2DefendPos.position;
        _head.GetComponent<Renderer>().enabled = false;
        var cols = _head.GetComponents<Collider>();
        foreach (var col in cols)
            col.enabled = false;
    }

    private void Spawn()
    {
        
        if (_trySpawnCounter >= _bossData.MaxTrySpawnCycles)
            _spawned = true;
        if (_spawnCounter >= _bossData.Phase2Spawn)
            _spawned = true;
        

        if (!CanSpawn()) return;

        // check how many enemies are on the selected platforms trigger area
        // if it is higher than a certain value then dont spawn at that platform
        // count up the try to spawn counter, if it is higher than X then set _spawned to true (we cant spawn)

        var rand = new System.Random();
        var index = rand.Next(0, _platformIds.Length);
        var pId = _platformIds[index];

        if (!CheckPlatform(pId))
        {
            return;
        }
        
        _arcs[pId].GetComponent<MeshFilter>().sharedMesh =
            _bossData.Enemy2.GetComponent<MeshFilter>().sharedMesh;

        _arcs[pId].GetComponent<EnemySpawn>().Enemy = _bossData.Enemy2;

        _arcs[pId].GetComponent<MeshRenderer>().enabled = true;
        _arcs[pId].GetComponent<SplineController>().FollowSpline();

        ++_spawnCounter;
        _spawnTimer = TimeBetweenSpawns;

        _bossData.PlayBossSpawnSound();

        _platformIds = _platformIds.Where(val => val != pId).ToArray();

    }

    private void UpdateTimers()
    {
        _spawnTimer -= Time.deltaTime;
        _timer -= Time.deltaTime;
    }

    private void PlayIdleSound()
    {
        _bossData.PlayBossIdleSound();
        _playing = true;
    }

    private bool CanPlayIdleSound()
    {
        return Random.value <= 0.01f && !_playing;
    }

    private int EnemiesOnPlatform(int id)
    {
        // this is the one that is wrong, we need to use index and not platform id?
        // we need to find the platform with the input id and then check how many enemies are on the platform
        foreach (var g in _spawnPoints)
        {
            if (g.GetComponent<PlatformID>().PlatformId == id)
                return g.GetComponentInChildren<EnemiesOnPlatform>().Amount;
        }
        return -1; // can mess up, might have to be a really high value instead of low value
    }

    private bool CheckPlatform(int pId)
    {
        if (EnemiesOnPlatform(pId) < _bossData.MaxEnemiesPerPlatfor) return true;

        _platformIds = _platformIds.Where(val => val != pId).ToArray();
        return false;
    }

    private bool CanSpawn()
    {
        return _spawnCounter < _bossData.Phase2Spawn && _spawnTimer <= 0.0f && !_spawned;
    }
}
