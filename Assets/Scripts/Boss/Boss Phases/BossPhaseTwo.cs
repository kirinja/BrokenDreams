using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseTwo : IBossPhaseState
{
    private IBossSubState _internalState;
    private BossBehaviour _bossData;

    public void Enter(BossBehaviour data)
    {
        //throw new System.NotImplementedException();
        _bossData = data;
        _bossData.PhasePlatforms[1].SetActive(true);
        _bossData.BossPhase2.SetActive(true);
        _bossData.Phase2Launch.SetActive(true);
        _internalState = new BossSubTwoIdle();
        _internalState.Enter(_bossData);
    }

    public IBossPhaseState Execute()
    {
        //throw new System.NotImplementedException();
        var state = _internalState.Execute();
        // switch internal state if needed
        if (state != null)
        {
            _internalState.Exit();
            _internalState = state;
            _internalState.Enter(_bossData);
        }
        return !Alive() ? null : new BossPhaseThree();
        /*if (!Alive())
            return null;
        else
        {
            return new BossPhaseOne();
        }
        Debug.Log(Alive());
        if (Alive())
            return null;
        else
        {
            return new BossPhaseOne();
        }*/
    }

    public void Exit()
    {
        //throw new System.NotImplementedException();

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject g in enemies)
        {
            Object.Destroy(g);
        }

        _bossData.PhasePlatforms[1].SetActive(false);
        _bossData.BossPhase2.SetActive(false);
        _bossData.Phase2Launch.SetActive(false);
    }

    public bool Alive()
    {
        //throw new System.NotImplementedException();
        return _internalState.Alive();
    }

    // this will be called via monobehaviour
    public void TakeDamage(int value)
    {
        _bossData.PlayBossDamageSound();
        _internalState.TakeDamage(value);
    }
}
