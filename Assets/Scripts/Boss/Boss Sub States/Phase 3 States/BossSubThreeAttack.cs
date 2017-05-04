using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BossSubThreeAttack : IBossSubState
{

    private BossBehaviour _bossData;
    private float _timer;
    private int _projCounter = 0;
    private float _attackTimer;

    private float _acidTimer;

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        //_timer = _bossData.StateSwitchTimer;
        _timer = new System.Random().Next((int)_bossData.MinStateSwitch, (int)_bossData.MaxStateSwitch); // HACK

        _attackTimer = _bossData.TimeBetweenShots;
        _projCounter = 0;
        _acidTimer = _bossData.AcidTimer;
    }

    public IBossSubState Execute()
    {
        // behaviour for spawning enemies
        // we cant spawn enemies  like this, it needs to happen once and then move back to idle, otherwise we're gonna spawn enemies every frame for X amount of time
        //Debug.Log("Spawn enemy 1 at random locations");

        _bossData.BossPhase3.GetComponent<SplineInterpolator>().enabled = true;

        if (_projCounter < _bossData.Phase3Projectiles && _attackTimer <= 0.0f)
        {
            // spawn projectiles and launch them at the player
            var spawnPos = GameObject.Find("LowerJaw").transform.position; // HACK
            var g = Object.Instantiate(_bossData.Projectiles, spawnPos, Quaternion.identity);
            g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, -1);
            //Debug.Log(g);

            _projCounter++;
            _attackTimer = _bossData.TimeBetweenShots;

            _bossData.PlayProjSound();
        }

        if (_acidTimer <= 0.0f)
        {
            var spawnPos = GameObject.Find("LowerJaw").transform.position; // HACK
            var g = Object.Instantiate(_bossData.Acid, spawnPos, _bossData.Acid.transform.rotation);
            g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, -1);

            _acidTimer = _bossData.AcidTimer;

            _bossData.PlaySpawnSound();
        }

        // use a _timer or something to determine when we should switch state

        _acidTimer -= Time.deltaTime;
        _attackTimer -= Time.deltaTime;
        _timer -= Time.deltaTime;

        if (!(_timer <= 0.0f)) return null;
        var r = Random.value;
        if (r <= 0.5f)
            return new BossSubThreePatrol();

        return new BossSubThreeIdle();
    }

    public void Exit()
    {
        //throw new System.NotImplementedException();
        _projCounter = 0;
    }

    public bool Alive()
    {
        return _bossData.HP <= _bossData.damageSwitchPhase3;
    }

    public void TakeDamage(int value)
    {
        _bossData.HP -= value;
    }
}
