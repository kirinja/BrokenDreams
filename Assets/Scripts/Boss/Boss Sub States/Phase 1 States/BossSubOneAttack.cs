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

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        _timer = _bossData.StateSwitchTimer;
        _spawned = false;
        _spawnedBox = false;
    }
    
    public IBossSubState Execute()
    {
        if (!_spawned)
        {
            var spawnPoints = GameObject.FindGameObjectsWithTag("Platform");

            var rand = new Random();

            // setup the platform ID's
            int[] arr = new int[spawnPoints.Length];
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                arr[i] = i;
            }

            for (int i = 0; i < _bossData.Phase1Spawn; i++)
            {
                var index  = rand.Next(0, arr.Length);
                var v = arr[index];
                Debug.Log("Spawn at platform ID " + v);
                var g = GameObject.Instantiate(_bossData.Enemy1, spawnPoints[v].transform.position + new Vector3(0, spawnPoints[v].transform.localScale.y / 2 + _bossData.Enemy1.transform.localScale.y / 2, -1) , Quaternion.identity);
                arr = arr.Where(val => val != v).ToArray();
                if (!_spawnedBox)
                {
                    // put box under the newly spawned enemy, disable collision and all that stuff
                    // also use the .setDrop on the enemy in question
                    var box = GameObject.Instantiate(_bossData.PushableBox, g.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity, g.transform);
                    box.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                    //box.transform.SetParent(g.transform);
                    box.GetComponent<Collider>().enabled = false;
                    box.GetComponent<Rigidbody>().useGravity = false;
                    _spawnedBox = true;
                }
            }
            _spawned = true;
        }

        // behaviour for spawning enemies
        Debug.Log("Phase 1 Attack State");
        // we cant spawn enemies  like this, it needs to happen once and then move back to idle, otherwise we're gonna spawn enemies every frame for X amount of time
        //Debug.Log("Spawn enemy 1 at random locations");

        // use a _timer or something to determine when we should switch state
        _timer -= Time.deltaTime;
        return _timer <= 0.0f ? new BossSubOneIdle() : null;
        //throw new System.NotImplementedException();
    }

    public void Exit()
    {
        //throw new System.NotImplementedException();
        _spawned = false;
        _spawnedBox = false;
    }

    public bool Alive()
    {
        return _bossData.HP <= _bossData.phase1;
    }

    public void TakeDamage(int value)
    {
        _bossData.HP -= value;
    }
}
