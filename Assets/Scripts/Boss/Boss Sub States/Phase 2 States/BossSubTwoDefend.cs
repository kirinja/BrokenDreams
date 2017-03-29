using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSubTwoDefend: IBossSubState
{

    private BossBehaviour _bossData;
    private GameObject _head;
    private float _timer;

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        _timer = _bossData.StateSwitchTimer; // TODO

        _head = GameObject.Find("Head");
    }

    public IBossSubState Execute()
    {
        Debug.Log("Phase 2 Defend");
        _head.transform.position = _bossData.Phase2DefendPos.position;
        _head.GetComponent<Renderer>().enabled = false;
        var cols = _head.GetComponents<Collider>();
        foreach (var col in cols)
            col.enabled = false;
        
        _timer -= Time.deltaTime;
        if (!(_timer <= 0.0f)) return null;

        var r = Random.value;
        if (r <= 0.5)
            return new BossSubTwoIdle();

        return new BossSubTwoAttack();
    }

    public void Exit()
    {
    }

    public bool Alive()
    {
        return _bossData.HP <= _bossData.damageSwitchPhase2;
    }

    public void TakeDamage(int value)
    {
        _bossData.HP -= value;
    }
}
