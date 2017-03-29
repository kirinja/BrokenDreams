using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSubTwoIdle : IBossSubState
{
    private BossBehaviour _bossData;
    private float _timer;

    public void Enter(BossBehaviour data)
    {
        //throw new System.NotImplementedException();
        _bossData = data;
        _timer = _bossData.StateSwitchTimer;
    }

    public IBossSubState Execute()
    {
        Debug.Log("Phase 2 Idle State");
        _timer -= Time.deltaTime;

        if (!(_timer <= 0.0f)) return null;
        var r = Random.value;
        if (r <= 0.5)
            return new BossSubTwoAttack();

        return new BossSubTwoDefend();
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
