﻿using System.Linq;
using UnityEngine;

public class BossSubOneAttack : IBossSubState
{
    private BossBehaviour _bossData;
    private float _timer;
    private bool _spawned;
    private bool _spawnedBox;
    private float _spawnTimer;
    private const float TimeBetweenSpawns = 1.0f;
    private int _spawnCounter;
    private int _trySpawnCounter;

    private int[] _platformIds;
    private GameObject[] _spawnPoints;
    private GameObject[] _arcs;

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        _timer = new System.Random().Next((int)_bossData.MinStateSwitch, (int)_bossData.MaxStateSwitch); // HACK
        _spawned = false;
        _spawnedBox = false;

        _spawnCounter = 0;
        _spawnTimer = TimeBetweenSpawns;
        _trySpawnCounter = 0;

        _spawnPoints = GameObject.FindGameObjectsWithTag("Platform");
        // this can fix itself if we remove the platforms that have MaxAmount of enemies on them already
        // we're gonna create the platformIds and then iterate over the platforms and check if they have more than 2 enemies, if so then remove that platform
        _platformIds = new int[_spawnPoints.Length];
        for (var i = 0; i < _spawnPoints.Length; i++)
            _platformIds[i] = i;

        // this should clear up the platform ids at the start
        // TODO how do we know which ID corresponds to which Platform. Need to think over this part of the code

        foreach (var id in _platformIds)
        {
            CheckPlatform(id);
        }

        _arcs = new GameObject[_bossData.Phase1Launch.transform.childCount];

        for (var i = 0; i < _bossData.Phase1Launch.transform.childCount; i++)
            _arcs[i] = _bossData.Phase1Launch.transform.GetChild(i).gameObject;
    }

    public IBossSubState Execute()
    {
        if (_platformIds.Length <= 0)
            return new BossSubOneIdle();
        //if (_trySpawnCounter >= _bossData.MaxTrySpawnCycles)
        //    _spawned = true;

        //if (!_spawned)
        //{
            Spawn();
        //}
        
        // behaviour for spawning enemies
        UpdateTimers();
        return _timer <= 0.0f ? new BossSubOneIdle() : null;
    }

    private void UpdateTimers()
    {
        _spawnTimer -= Time.deltaTime;
        _timer -= Time.deltaTime;
    }

    private void Spawn()
    {
        if (_trySpawnCounter >= _bossData.MaxTrySpawnCycles) _spawned = true;

        if (!CanSpawn()) return;
        

        var rand = new System.Random();
        var index = rand.Next(0, _platformIds.Length);
        var pId = _platformIds[index];
        

        // TODO fix bug here. Spawning is wonky right now and we can spawn multiple on the same platforms
        // we should try a different platform to spawn on if we cant do this one (recurssion maybe?)
        // there is also the issue that we can spawn more than the max value set in Boss Behaviour
        // if a platform is unfit for spawning, then remove that platform from the platform IDs?
        // still buggy but takes longer time before it breaks down
        if (!CheckPlatform(pId))
        {
            return;
        }


        if (!_spawnedBox)
        {
            // create a cube, assign the pushable color to it, disable all stuff that has with collision, asign correct position ans scale, set previously spawned enemy as parent
            var b = GameObject.CreatePrimitive(PrimitiveType.Cube);
            b.GetComponent<Renderer>().sharedMaterial =
                _bossData.PushableBox.GetComponent<Renderer>().sharedMaterial;
            b.GetComponent<Collider>().enabled = false;
            b.transform.position = _arcs[pId].transform.position + new Vector3(0, 0.5f, 0);
            b.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            b.transform.SetParent(_arcs[pId].transform);
            _arcs[pId].GetComponent<EnemySpawn>().PushableBox = _bossData.PushableBox;
            _spawnedBox = true;
        }

        _arcs[pId].GetComponent<MeshFilter>().sharedMesh = _bossData.Enemy1.GetComponent<MeshFilter>().sharedMesh;
        _arcs[pId].GetComponent<EnemySpawn>().Enemy = _bossData.Enemy1;
        _arcs[pId].GetComponent<MeshRenderer>().enabled = true;
        _arcs[pId].GetComponent<SplineController>().FollowSpline();

        _spawnCounter++;
        _spawnTimer = TimeBetweenSpawns;

        _bossData.PlayBossSpawnSound();
        _platformIds = _platformIds.Where(val => val != pId).ToArray();


        if (_spawnCounter >= _bossData.Phase1Spawn)
            _spawned = true;
    }

    private bool CanSpawn()
    {
        return _spawnCounter < _bossData.Phase1Spawn && _spawnTimer <= 0.0f && !_spawned;
    }

    public void Exit()
    {
        _spawned = false;
        _spawnedBox = false;
    }

    public bool Alive()
    {
        return _bossData.HP <= _bossData.damageSwitchPhase1;
    }

    public void TakeDamage(int value)
    {
        _bossData.HP -= value;
    }

    private int EnemiesOnPlatform(int id)
    {
        return _spawnPoints[id].GetComponentInChildren<EnemiesOnPlatform>().Amount;
    }

    private bool CheckPlatform(int pId)
    {
        if (EnemiesOnPlatform(pId) < _bossData.MaxEnemiesPerPlatfor) return true;
        //_trySpawnCounter++;
        // remove the ID from the list
        _platformIds = _platformIds.Where(val => val != pId).ToArray();
        foreach (var i in _platformIds)
            Debug.Log(i + " - " + _spawnPoints[i].name);

        return false;
        // restart Execute command
        //Execute();
    }
}
