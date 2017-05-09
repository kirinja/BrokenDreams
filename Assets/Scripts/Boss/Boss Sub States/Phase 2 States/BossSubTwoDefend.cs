using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

        _spawned = false;

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

        _bossData.PlayPhaseTwoDefendSound();
    }

    public IBossSubState Execute()
    {
        HideHead();

        // if we can get rid of the _spawned check here then we can use this line of code
        // sometimes the boss only spawns 1 or 2 enemies instead of the full 3, so the _spawned is never set to true and we can never progress
        // unsure what triggers this though since I can't seem to recreate it reliably
        if (_spawned && EnemiesKilled())
        {
            return new BossSubTwoAttack();
        }

        // here we need to add so we cant flood the scene
        Spawn();

        // give up on trying to spawn (possibly move out)
        

        if (CanPlayIdleSound())
            PlayIdleSound();

        UpdateTimers();

        if (!(_timer <= 0.0f)) return null;
        
        //if (Random.value <= 0.5)
        //return new BossSubTwoIdle();
        return null;
        //return new BossSubTwoAttack();
    }

    public void Exit()
    {
        _spawned = false;
        _spawnCounter = 0;
        _spawnTimer = TimeBetweenSpawns;
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
         // lerp this shit
        _head.transform.position = Vector3.Lerp(_head.transform.position, _bossData.Phase2DefendPos.position, 0.10f);
        _head.transform.localScale = Vector3.Lerp(_head.transform.localScale, new Vector3(0.1f, 0.1f, 0.1f), 0.25f);
        //_head.transform.position = _bossData.Phase2DefendPos.position;
        //_head.GetComponent<Renderer>().enabled = false;
        var cols = _head.GetComponents<Collider>();
        foreach (var col in cols)
            col.enabled = false;
        
    }

    private void Spawn()
    {
        if (_spawnCounter >= _bossData.Phase2Spawn)
            _spawned = true;
        

        if (!CanSpawn()) return;

        // check how many enemies are on the selected platforms trigger area
        // if it is higher than a certain value then dont spawn at that platform
        // count up the try to spawn counter, if it is higher than X then set _spawned to true (we cant spawn)
        //var pId = -1;
        //var index = -1;
        try
        {
            if (_platformIds.Length > 0)
            {
                var rand = new System.Random();
                var index = rand.Next(0, _platformIds.Length);
                var pId = _platformIds[index];
                if (!CheckPlatform(pId)) return;

                _arcs[pId].GetComponent<EnemySpawn>().Enemy = _bossData.Enemy2;

                //_arcs[pId].GetComponent<MeshRenderer>().enabled = true;
                _arcs[pId].GetComponentInChildren<SpriteRenderer>().enabled = true;
                _arcs[pId].GetComponent<SplineController>().FollowSpline();

                ++_spawnCounter;
                _spawnTimer = TimeBetweenSpawns;

                _bossData.PlaySpawnSound();

                // remove the platform from the array (since we have spawned here already) ? unneccessary?
                _platformIds = _platformIds.Where(val => val != pId).ToArray();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error when trying to spawn enemies in phase2 - state defend \t" + e.Message);
        }

    }

    private void UpdateTimers()
    {
        _spawnTimer -= Time.deltaTime;
        _timer -= Time.deltaTime;
    }

    private void PlayIdleSound()
    {
        _bossData.PlayIdleSound();
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
        return _bossData.MaxEnemiesPerPlatfor + 1;
    }

    private bool CheckPlatform(int pId)
    {
        if (EnemiesOnPlatform(pId) < _bossData.MaxEnemiesPerPlatfor) return true;

        // if the amount of enemies on the platform is below the max value then we remove this platform ID from the platform array
        _platformIds = _platformIds.Where(val => val != pId).ToArray();
        return false;
    }

    private bool CanSpawn()
    {
        bool condition1 = _spawnTimer <= 0.0f;
        //bool condition2 = _spawnCounter < _bossData.Phase2Spawn;
        bool condition3 = !_spawned;
        //bool condition4 = _platformIds.Length >= 0;
        return condition1 && condition3;// && condition4;
    }

    private bool EnemiesKilled()
    {
        //Enemy02(Clone)
        var enemies = GameObject.FindObjectsOfType<Enemy02behaviour3D>();

        return enemies.Length <= 0;
    }
}
