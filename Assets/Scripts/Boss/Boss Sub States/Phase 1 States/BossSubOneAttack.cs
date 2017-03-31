using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class BossSubOneAttack : IBossSubState{

    private BossBehaviour _bossData;
    private float _timer;
    private bool _spawned;
    private bool _spawnedBox;
    private float _spawnTimer;
    private const float TimeBetweenSpawns = 1.0f;
    private int _spawnCounter;

    private int[] _platformIds;
    private GameObject[] _spawnPoints;

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        _timer = _bossData.StateSwitchTimer; // TODO
        _spawned = false;
        _spawnedBox = false;

        _spawnCounter = 0;
        _spawnTimer = TimeBetweenSpawns;

        _spawnPoints = GameObject.FindGameObjectsWithTag("Platform");
        _platformIds = new int[_spawnPoints.Length];
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            _platformIds[i] = i;
        }
    }
    
    public IBossSubState Execute()
    {
        Debug.Log("Phase 1 Attack");
        if (!_spawned)
        {
            //var spawnPoints = GameObject.FindGameObjectsWithTag("Platform");

            var rand = new Random();

            //// setup the platform ID's
            //int[] arr = new int[spawnPoints.Length];
            //for (int i = 0; i < spawnPoints.Length; i++)
            //{
            //    arr[i] = i;
            //}

            //for (int i = 0; i < _bossData.Phase1Spawn; i++)
            //{
            //    var index  = rand.Next(0, arr.Length);
            //    var v = arr[index];
            //    Debug.Log("Spawn at platform ID " + v);
            //    var g = GameObject.Instantiate(_bossData.Enemy1, spawnPoints[v].transform.position + new Vector3(0, spawnPoints[v].transform.localScale.y / 2 + _bossData.Enemy1.transform.localScale.y / 2, -1) , Quaternion.identity);
            //    arr = arr.Where(val => val != v).ToArray();
                

                if (_spawnCounter < _bossData.Phase1Spawn && _spawnTimer <= 0.0f)
                {
                    var index = rand.Next(0, _platformIds.Length);
                    var v = _platformIds[index];
                    Debug.Log("Spawn at platform ID " + v);
                    var g = GameObject.Instantiate(_bossData.Enemy1, _spawnPoints[v].transform.position + new Vector3(0, _spawnPoints[v].transform.localScale.y / 2 + _bossData.Enemy1.transform.localScale.y / 2, -1), Quaternion.identity);
                    _platformIds = _platformIds.Where(val => val != v).ToArray();

                    if (!_spawnedBox)
                    {
                        // put box under the newly spawned enemy, disable collision and all that stuff
                        // also use the .setDrop on the enemy in question

                        // create a cube, assign the pushable color to it, disable all stuff that has with collision, asign correct position ans scale, set previously spawned enemy as parent
                        var b = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        b.GetComponent<Renderer>().sharedMaterial = _bossData.PushableBox.GetComponent<Renderer>().sharedMaterial;
                        b.GetComponent<Collider>().enabled = false;
                        b.transform.position = g.transform.position + new Vector3(0, 0.5f, 0);
                        b.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                        b.transform.SetParent(g.transform);
                        _spawnedBox = true;
                    }

                    _spawnCounter++;
                    _spawnTimer = TimeBetweenSpawns;

                    _bossData.PlayBossSpawnSound();
                }

            //}
        }
        if (_spawnCounter >= _bossData.Phase1Spawn)
            _spawned = true;

        // behaviour for spawning enemies

        _spawnTimer -= Time.deltaTime;
        _timer -= Time.deltaTime;
        return _timer <= 0.0f ? new BossSubOneIdle() : null;
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
}
