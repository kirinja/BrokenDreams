using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSubTwoIdle : IBossSubState
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
        Debug.Log("Phase 2 Idle State");
        timer -= Time.deltaTime;
        return timer <= 0.0f ? new BossSubTwoAttack() : null;
        //return null;
    }

    public void Exit()
    {
        //throw new System.NotImplementedException();
    }

    public bool Alive()
    {
        //throw new System.NotImplementedException();
        return _bossData.HP <= _bossData.phase2;
    }

    public void TakeDamage(int value)
    {
        _bossData.HP -= value;
    }
}
