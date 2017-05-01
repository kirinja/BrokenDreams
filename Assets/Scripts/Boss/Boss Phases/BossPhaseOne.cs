using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseOne : IBossPhaseState
{
    private IBossSubState _internalState;
    private BossBehaviour _bossData;

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        _bossData.PhasePlatforms[0].SetActive(true);
        _bossData.BossPhase1.SetActive(true);
        _bossData.Phase1Launch.SetActive(true);
        _internalState = new BossSubOneAttack();
        _internalState.Enter(_bossData);
    }

    public IBossPhaseState Execute()
    {
        
        var state = _internalState.Execute();
        // switch internal state if needed
        if (state != null)
        {
            _internalState.Exit();
            _internalState = state;
            _internalState.Enter(_bossData);
        }
        return !Alive() ? null : new BossPhaseTwo();
    }

    public void Exit()
    {
        _bossData.PhasePlatforms[0].SetActive(false);
        _bossData.BossPhase1.SetActive(false);
        _bossData.Phase1Launch.SetActive(false);
        // we need to kill all the objects we have spawned in this phase as clean up
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject g in enemies)
        {
            Object.Destroy(g);
        }

        var boxes = GameObject.FindGameObjectsWithTag("Movable Object");
        foreach (GameObject g in boxes)
        {
            Object.Destroy(g);
        }

        // TODO Make this neater
        //Object.Instantiate(_bossData.HealthPickUp, new Vector3(-6.25f, 1.5f, -1f), Quaternion.identity);
        //Object.Instantiate(_bossData.HealthPickUp, new Vector3(-2.25f, 1.5f, -1f), Quaternion.identity);
    }

    public bool Alive()
    {
        return _internalState.Alive();
    }

    // this will be called via monobehaviour
    public void TakeDamage(int value)
    {
        _bossData.PlayDamageSound();
        _internalState.TakeDamage(value);
    }
}
