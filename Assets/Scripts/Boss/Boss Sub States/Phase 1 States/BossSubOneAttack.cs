using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class BossSubOneAttack : IBossSubState
{

    private BossBehaviour _bossData;
    private float _timer;
    private bool _spawned;
    private bool _spawnedBox;
    private float _spawnTimer;
    private const float TimeBetweenSpawns = 1.0f;
    private int _spawnCounter;

    private int[] _platformIds;

    private GameObject[] _arcs;

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        //_timer = _bossData.StateSwitchTimer; // TODO
        _timer = new Random().Next((int)_bossData.MinStateSwitch, (int)_bossData.MaxStateSwitch); // HACK
        _spawned = false;
        _spawnedBox = false;

        _spawnCounter = 0;
        _spawnTimer = TimeBetweenSpawns;

        var _spawnPoints = GameObject.FindGameObjectsWithTag("Platform");
        _platformIds = new int[_spawnPoints.Length];
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            _platformIds[i] = i;
        }

        _arcs = new GameObject[_bossData.Phase1Launch.transform.childCount];
        for (int i = 0; i < _bossData.Phase1Launch.transform.childCount; i++)
        {
            _arcs[i] = _bossData.Phase1Launch.transform.GetChild(i).gameObject;
        }
    }

    public IBossSubState Execute()
    {
        if (!_spawned)
        {
            var rand = new Random();
            
            if (_spawnCounter < _bossData.Phase1Spawn && _spawnTimer <= 0.0f)
            {
                var index = rand.Next(0, _platformIds.Length);
                var v = _platformIds[index];
                //var g = GameObject.Instantiate(_bossData.Enemy1, _spawnPoints[v].transform.position + new Vector3(0, _spawnPoints[v].transform.localScale.y / 2 + _bossData.Enemy1.transform.localScale.y / 2, -1), Quaternion.identity);

                _platformIds = _platformIds.Where(val => val != v).ToArray();

                if (!_spawnedBox)
                {
                    // create a cube, assign the pushable color to it, disable all stuff that has with collision, asign correct position ans scale, set previously spawned enemy as parent
                    var b = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    b.GetComponent<Renderer>().sharedMaterial = _bossData.PushableBox.GetComponent<Renderer>().sharedMaterial;
                    b.GetComponent<Collider>().enabled = false;
                    b.transform.position = _arcs[v].transform.position + new Vector3(0, 0.5f, 0);
                    b.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                    b.transform.SetParent(_arcs[v].transform);
                    //_arcs[v].GetComponent<EnemySpawn>().Enemy.GetComponent<Enemy01Behaviour>().Drop = _bossData.PushableBox;
                    _arcs[v].GetComponent<EnemySpawn>().PushableBox = _bossData.PushableBox;
                    _spawnedBox = true;
                }

                _arcs[v].GetComponent<MeshFilter>().sharedMesh = _bossData.Enemy1.GetComponent<MeshFilter>().sharedMesh;
                _arcs[v].GetComponent<EnemySpawn>().Enemy = _bossData.Enemy1;
                _arcs[v].GetComponent<MeshRenderer>().enabled = true;
                _arcs[v].GetComponent<SplineController>().FollowSpline();

                _spawnCounter++;
                _spawnTimer = TimeBetweenSpawns;

                _bossData.PlayBossSpawnSound();
            }
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

    void Launch(Transform projectile, Transform target, float angle)
    {
        Vector3 pos = projectile.position;
        Vector3 tar = target.position;

        float distance = Vector3.Distance(pos, tar);

        projectile.LookAt(target);

        float Vi = Mathf.Sqrt(distance * -Physics.gravity.y / (Mathf.Sin(Mathf.Deg2Rad * angle * 2)));
        float Vy, Vz;

        Vy = Vi * Mathf.Sin(Mathf.Deg2Rad * angle);
        Vz = Vi * Mathf.Cos(Mathf.Deg2Rad * angle);

        Vector3 localVel = new Vector3(0f, Vy, Vz);

        Vector3 globalVel = projectile.TransformDirection(localVel);

        projectile.position += globalVel * Time.deltaTime;
    }
}
