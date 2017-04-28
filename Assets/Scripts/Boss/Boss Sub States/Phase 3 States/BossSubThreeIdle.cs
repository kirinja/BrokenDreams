using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSubThreeIdle : IBossSubState
{

    private BossBehaviour _bossData;
    private float _timer;
    private bool _playing;

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        //_timer = _bossData.StateSwitchTimer;
        _timer = (_bossData.MinStateSwitch/2); //new System.Random().Next((int)_bossData.MinStateSwitch, (int)_bossData.MaxStateSwitch); // HACK

        _playing = false;
    }

    public IBossSubState Execute()
    {
        // behaviour for spawning enemies
        // we cant spawn enemies  like this, it needs to happen once and then move back to idle, otherwise we're gonna spawn enemies every frame for X amount of time
        //Debug.Log("Spawn enemy 1 at random locations");

        GameObject.Find("BossPhase3").GetComponent<SplineInterpolator>().enabled = false;

        var r = Random.value;
        if (r <= 0.01f && !_playing)
        {
            _bossData.PlayBossIdleSound();
            _playing = true;
        }


        // use a _timer or something to determine when we should switch state
        _timer -= Time.deltaTime;

        if (!(_timer <= 0.0f)) return null;
        var t = Random.value;
        if (t <= 0.5f)
            return new BossSubThreeAttack();

        return new BossSubThreePatrol();
    }

    public void Exit()
    {
        //throw new System.NotImplementedException();
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
