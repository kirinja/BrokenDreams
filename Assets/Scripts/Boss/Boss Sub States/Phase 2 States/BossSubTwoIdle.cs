﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSubTwoIdle : IBossSubState
{
    private BossBehaviour _bossData;
    private float _timer;
    private bool _playing;

    private int _phaseStartHp;

    public void Enter(BossBehaviour data)
    {
        //throw new System.NotImplementedException();
        _bossData = data;
        _phaseStartHp = _bossData.HP;
        //_timer = _bossData.StateSwitchTimer;
        _timer = new System.Random().Next((int)_bossData.MinStateSwitch, (int)_bossData.MaxStateSwitch); // HACK

        _playing = false;
    }

    public IBossSubState Execute()
    {
        // HACK Y HACKY
        if (_bossData.HP < _phaseStartHp)
        {
            // boss was damaged, go to defend state
            return new BossSubTwoDefend();
        }
        _timer -= Time.deltaTime;

        var r = Random.value;
        if (r <= 0.01f && !_playing)
        {
            _bossData.PlayIdleSound();
            _playing = true;
        }

        if (!(_timer <= 0.0f)) return null;
        //var t = Random.value;
        //if (t <= 0.5)
        return new BossSubTwoAttack();

        //return new BossSubTwoDefend();
    }

    public void Exit()
    {
        //throw new System.NotImplementedException();
    }

    public bool Alive()
    {
        //throw new System.NotImplementedException();
        return _bossData.HP <= _bossData.damageSwitchPhase2;
    }

    public void TakeDamage(int value)
    {
        _bossData.HP -= value;
    }
}
