using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BossSubThreeAttack : IBossSubState
{

    private BossBehaviour _bossData;
    private float timer;
    private int _projCounter = 0;
    private const float TimeBetweenShots = 0.5f;
    private float _attackTimer;

    private float _acidTimer;

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        timer = _bossData.StateSwitchTimer;
        _attackTimer = TimeBetweenShots;
        _projCounter = 0;
        _acidTimer = _bossData.AcidTimer;
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
            g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, -1);
            //Debug.Log(g);

            _projCounter++;
            _attackTimer = TimeBetweenShots;
        }

        if (_acidTimer <= 0.0f)
        {
            Debug.Log("Try to drop acid");
            var g = Object.Instantiate(_bossData.Acid, _bossData.BossPhase3.transform.position, _bossData.Acid.transform.rotation);
            g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, -1);

            _acidTimer = _bossData.AcidTimer;
        }

        // use a timer or something to determine when we should switch state

        _acidTimer -= Time.deltaTime;
        _attackTimer -= Time.deltaTime;
        timer -= Time.deltaTime;
        return timer <= 0.0f ? new BossSubThreePatrol() : null;
        //throw new System.NotImplementedException();
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
