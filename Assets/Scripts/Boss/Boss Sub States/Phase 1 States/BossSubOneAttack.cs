using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSubOneAttack : IBossSubState{

    private BossBehaviour _bossData;
    private float timer;

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        timer = _bossData.StateSwitchTimer;
    }

    public IBossSubState Execute()
    {
        // behaviour for spawning enemies
        Debug.Log("Phase 1 Attack State");
        // we cant spawn enemies  like this, it needs to happen once and then move back to idle, otherwise we're gonna spawn enemies every frame for X amount of time
        //Debug.Log("Spawn enemy 1 at random locations");

        // use a timer or something to determine when we should switch state
        timer -= Time.deltaTime;
        return timer <= 0.0f ? new BossSubOneIdle() : null;
        //throw new System.NotImplementedException();
    }

    public void Exit()
    {
        //throw new System.NotImplementedException();
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
