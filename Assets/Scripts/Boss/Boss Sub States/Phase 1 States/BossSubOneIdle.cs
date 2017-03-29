using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSubOneIdle : IBossSubState
{
    private BossBehaviour _bossData;
    private float timer;

    public void Enter(BossBehaviour data)
    {
        //throw new System.NotImplementedException();
        _bossData = data;
        timer = _bossData.StateSwitchTimer;
    }

    public IBossSubState Execute()
    {
        //throw new System.NotImplementedException();
        Debug.Log("Phase 1 Idle State");
        timer -= Time.deltaTime;
        return timer <= 0.0f  ? new BossSubOneAttack() : null;
        //return null;
    }

    public void Exit()
    {
        //throw new System.NotImplementedException();
    }

    public bool Alive()
    {
        //throw new System.NotImplementedException();
        return _bossData.HP <= _bossData.damageSwitchPhase1;
    }

    public void TakeDamage(int value)
    {
        _bossData.HP -= value;
    }
}
