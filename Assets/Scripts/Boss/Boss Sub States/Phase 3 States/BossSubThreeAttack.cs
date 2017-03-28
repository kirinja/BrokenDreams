using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSubThreeAttack : IBossSubState
{

    private BossBehaviour _bossData;
    private float timer;
    private bool _attacked;
    private int _projCounter = 0;
    private const float TimeBetweenShots = 1.0f;
    private float _attackTimer;

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        timer = _bossData.StateSwitchTimer;
        _attackTimer = TimeBetweenShots;
        _attacked = false;
        _projCounter = 0;
    }

    public IBossSubState Execute()
    {
        // behaviour for spawning enemies
        Debug.Log("Phase 3 Attack State");
        // we cant spawn enemies  like this, it needs to happen once and then move back to idle, otherwise we're gonna spawn enemies every frame for X amount of time
        //Debug.Log("Spawn enemy 1 at random locations");

        GameObject.Find("BossPhase3").GetComponent<SplineInterpolator>().enabled = true;

        if (_projCounter < _bossData.Phase3Projectiles && _attackTimer <= 0.0f)
        {
            Debug.Log("Try to spawn projectiles");
            // spawn projectiles and launch them at the player
            var g = Object.Instantiate(_bossData.Projectiles, _bossData.BossPhase3.transform.position, Quaternion.identity);
            Debug.Log(g);

            _projCounter++;
            _attackTimer = TimeBetweenShots;
        }

        // use a timer or something to determine when we should switch state
        _attackTimer -= Time.deltaTime;
        timer -= Time.deltaTime;
        return timer <= 0.0f ? new BossSubThreePatrol() : null;
        //throw new System.NotImplementedException();
    }

    public void Exit()
    {
        //throw new System.NotImplementedException();
        _attacked = false;
        _projCounter = 0;
    }

    public bool Alive()
    {
        return _bossData.HP <= _bossData.phase3;
    }

    public void TakeDamage(int value)
    {
        _bossData.HP -= value;
    }
}
