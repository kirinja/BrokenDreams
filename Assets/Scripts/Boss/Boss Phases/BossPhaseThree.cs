using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseThree : IBossPhaseState
{
    private IBossSubState _internalState;
    private BossBehaviour _bossData;

    public void Enter(BossBehaviour data)
    {
        //throw new System.NotImplementedException();
        _bossData = data;
        _bossData.PhasePlatforms[2].SetActive(true);
        _bossData.BossPhase3.SetActive(true);
        _internalState = new BossSubThreePatrol();
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
        return null;
    }

    public void Exit()
    {
        //throw new System.NotImplementedException();

        _bossData.PhasePlatforms[2].SetActive(false);
        _bossData.BossPhase3.SetActive(false);
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
