using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSubTwoDefend: IBossSubState
{

    private BossBehaviour _bossData;
    private GameObject _head;
    private float timer;

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        timer = _bossData.StateSwitchTimer;

        _head = GameObject.Find("Head");
    }

    public IBossSubState Execute()
    {
        // behaviour for spawning enemies
        Debug.Log("Phase 2 Defend State");
        // we cant spawn enemies  like this, it needs to happen once and then move back to idle, otherwise we're gonna spawn enemies every frame for X amount of time
        //Debug.Log("Spawn enemy 1 at random locations");

        //_bossData.Phase2AttackPos.position;

        _head.transform.position = _bossData.Phase2DefendPos.position;
        _head.SetActive(false);
        var cols = _head.GetComponents<Collider>();
        foreach (Collider col in cols)
            col.enabled = false;

        // use a timer or something to determine when we should switch state
        timer -= Time.deltaTime;
        return timer <= 0.0f ? new BossSubTwoIdle() : null;
        //throw new System.NotImplementedException();
    }

    public void Exit()
    {
        //throw new System.NotImplementedException();
    }

    public bool Alive()
    {
        return _bossData.HP <= _bossData.phase2;
    }

    public void TakeDamage(int value)
    {
        _bossData.HP -= value;
    }
}
