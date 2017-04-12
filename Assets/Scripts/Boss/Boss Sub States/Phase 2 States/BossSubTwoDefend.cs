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
    private bool canSpawn = true;

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
        _head.transform.position = _bossData.Phase2DefendPos.position;
        _head.GetComponent<Renderer>().enabled = false;
        var cols = _head.GetComponents<Collider>();
        foreach (var col in cols)
            col.enabled = false;

        if (_trySpawnCounter >= _bossData.MaxTrySpawnCycles)
            _spawned = true;

        // here we need to add so we cant flood the scene
        if (!_spawned)
        {
            var rand = new System.Random();
            
            if (_spawnCounter < _bossData.Phase2Spawn && _spawnTimer <= 0.0f)
            {
                // check how many enemies are on the selected platforms trigger area
                // if it is higher than a certain value then dont spawn at that platform
                // count up the try to spawn counter, if it is higher than X then set _spawned to true (we cant spawn)
                var index = rand.Next(0, _platformIds.Length);
                var v = _platformIds[index];

                if (_spawnPoints[v].GetComponentInChildren<EnemiesOnPlatform>().Amount >= _bossData.MaxEnemiesPerPlatfor)
                {
                    _trySpawnCounter++;
                    if (_trySpawnCounter >= _bossData.MaxTrySpawnCycles)
                        _spawned = true;
                    // might get stuck in a loop here

                    //Debug.Log("cant spawn since platform is full " + _trySpawnCounter + " - " + _spawned);
                    canSpawn = false;
                    //return null;
                }

                if (canSpawn)
                {
                    var childCount = _bossData.NavmeshTargets.transform.childCount;
                    Transform[] childs = new Transform[childCount];
                    for (int i = 0; i < childCount; i++)
                    {
                        childs[i] = _bossData.NavmeshTargets.transform.GetChild(i);
                    }

                    _arcs[v].GetComponent<MeshFilter>().sharedMesh =
                        _bossData.Enemy2.GetComponent<MeshFilter>().sharedMesh;

                    _bossData.Enemy2.GetComponent<Enemy02behaviour3D>().retreatPoints = childs;
                    _arcs[v].GetComponent<EnemySpawn>().Enemy = _bossData.Enemy2;

                    _arcs[v].GetComponent<MeshRenderer>().enabled = true;
                    _arcs[v].GetComponent<SplineController>().FollowSpline();

                    ++_spawnCounter;
                    _spawnTimer = TimeBetweenSpawns;

                    _bossData.PlayBossSpawnSound();

                    _platformIds = _platformIds.Where(val => val != v).ToArray();
                }
            }
        }

        if (_spawnCounter >= _bossData.Phase2Spawn)
            _spawned = true;

        _spawnTimer -= Time.deltaTime;

        var r = Random.value;
        if (r <= 0.01f && !_playing)
        {
            _bossData.PlayBossIdleSound();
            _playing = true;
        }

        _timer -= Time.deltaTime;
        if (!(_timer <= 0.0f)) return null;

        var t = Random.value;
        if (t <= 0.5)
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
}
