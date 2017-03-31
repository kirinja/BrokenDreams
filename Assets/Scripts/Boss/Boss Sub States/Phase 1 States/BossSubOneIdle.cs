using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSubOneIdle : IBossSubState
{
    private BossBehaviour _bossData;
    private float _timer;
    private bool _playing;

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        //_timer = _bossData.StateSwitchTimer; //TODO
        _timer = new System.Random().Next((int)_bossData.MinStateSwitch, (int)_bossData.MaxStateSwitch); // HACK
        Debug.Log(_timer);

        _playing = false;
    }

    public IBossSubState Execute()
    {
        var r = Random.value;
        if (r <= 0.01f && !_playing)
        {
            _bossData.PlayBossIdleSound();
            _playing = true;
        }

        _timer -= Time.deltaTime;
        return _timer <= 0.0f  ? new BossSubOneAttack() : null;
    }

    public void Exit()
    {
        _playing = false;
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
