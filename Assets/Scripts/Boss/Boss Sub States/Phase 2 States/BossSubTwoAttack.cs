using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossSubTwoAttack : IBossSubState
{

    private BossBehaviour _bossData;
    private GameObject _head;
    private bool _spawned;
    private float _timer;

    private int _projCounter = 0;
    private const float TimeBetweenShots = 0.5f;
    private float _projTimer;

    private float _spawnTimer;
    private const float TimeBetweenSpawns = 1.0f;
    private int _spawnCounter;

    private int[] _platformIds;

    private GameObject[] _arcs;

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        //_timer = _bossData.StateSwitchTimer; // TODO
        _timer = new System.Random().Next((int)_bossData.MinStateSwitch, (int)_bossData.MaxStateSwitch); // HACK
        Debug.Log(_timer);

        _projTimer = TimeBetweenShots;

        _head = GameObject.Find("Head");

        _spawnCounter = 0;
        _spawnTimer = TimeBetweenShots;

        var spawnPoints = GameObject.FindGameObjectsWithTag("Platform");
        _platformIds = new int[spawnPoints.Length];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            _platformIds[i] = i;
        }

        //_arcs = _bossData.Phase2Launch;

        _arcs = new GameObject[_bossData.Phase2Launch.transform.childCount];
        for (int i = 0; i < _bossData.Phase2Launch.transform.childCount; i++)
        {
            _arcs[i] = _bossData.Phase2Launch.transform.GetChild(i).gameObject;
        }
    }

    public IBossSubState Execute()
    {
        _head.transform.position = _bossData.Phase2AttackPos.position;
        _head.GetComponent<Renderer>().enabled = true;
        var cols = _head.GetComponents<Collider>();
        foreach (var col in cols)
            col.enabled = true;

        if (!_spawned)
        {
            //var spawnPoints = GameObject.FindGameObjectsWithTag("Wall");

            var rand = new System.Random();

            //// setup the platform ID's
            //int[] arr = new int[spawnPoints.Length];
            //for (int i = 0; i < spawnPoints.Length; i++)
            //{
            //    arr[i] = i;
            //}

            // TODO this is broken, we need an enemy2 prefab but an enemy2 needs target to patrol between, which isnt included in the prefab
            // TODO enemy2 also need a projectile which isnt included in the prefab
            // TODO need to look over enemy2 and make it self-contained, or at least make it find the data it needs to work (cant manually assign when instanciating
            //for (int i = 0; i < _bossData.Phase2Spawn; i++)
            //{
            //    var index = rand.Next(0, arr.Length);
            //    var v = arr[index];
            //    Debug.Log("Spawn at platform ID " + v);
            //    Object.Instantiate(_bossData.Enemy2,
            //        spawnPoints[v].transform.position +
            //        new Vector3(0,
            //            spawnPoints[v].transform.localScale.y / 2 + _bossData.Enemy1.transform.localScale.y / 2, -1),
            //        Quaternion.identity);
            //    arr = arr.Where(val => val != v).ToArray();
            //}

            if (_spawnCounter < _bossData.Phase2Spawn && _spawnTimer <= 0.0f)
            {
                var index = rand.Next(0, _platformIds.Length);
                var v = _platformIds[index];
                //Debug.Log("Spawn at platform ID " + v);
                //var g = GameObject.Instantiate(_bossData.Enemy2, _spawnPoints[v].transform.position + new Vector3(0, _spawnPoints[v].transform.localScale.y / 2 + _bossData.Enemy2.transform.localScale.y / 2, -1), Quaternion.identity);
                
                _platformIds = _platformIds.Where(val => val != v).ToArray();

                _arcs[v].GetComponent<MeshFilter>().sharedMesh =_bossData.Enemy2.GetComponent<MeshFilter>().sharedMesh;
                _arcs[v].GetComponent<EnemySpawn>().Enemy = _bossData.Enemy2;
                _arcs[v].GetComponent<MeshRenderer>().enabled = true;
                _arcs[v].GetComponent<SplineController>().FollowSpline();

                _spawnCounter++;
                _spawnTimer = TimeBetweenSpawns;

                _bossData.PlayBossSpawnSound();
            }
        }

        if (_projCounter < _bossData.Phase3Projectiles && _projTimer <= 0.0f)
        {
            // spawn projectiles and launch them at the player
            var g = Object.Instantiate(_bossData.Projectiles, _bossData.BossPhase2.transform.position,
                Quaternion.identity);
            g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, -1);

            _projCounter++;
            _projTimer = TimeBetweenShots;

            _bossData.PlayBossProjSound();
        }

        if (_spawnCounter >= _bossData.Phase2Spawn)
            _spawned = true;

        _spawnTimer -= Time.deltaTime;
        _projTimer -= Time.deltaTime;
        _timer -= Time.deltaTime;

        if (!(_timer <= 0.0f)) return null;
        var r = Random.value;
        if (r <= 0.5f)
            return new BossSubTwoDefend();

        return new BossSubTwoIdle();
    }

    public void Exit()
    {
        _spawned = false;
    }

    public bool Alive()
    {
        return _bossData.HP <= _bossData.damageSwitchPhase2;
    }

    public void TakeDamage(int value)
    {
        _bossData.HP -= value;
    }
}
