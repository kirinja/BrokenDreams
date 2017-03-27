using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseTwo : IBossPhaseState
{
    private IBossSubState _internalState;
    private BossBehaviour _bossData;

    public void Enter(BossBehaviour data)
    {
        Debug.Log("Boss Phase 2");
        //throw new System.NotImplementedException();
        _bossData = data;
        _bossData.PhasePlatforms[1].SetActive(true);
        _internalState = new BossSubTwoIdle(); // TODO FIX
        _internalState.Enter(_bossData);
    }

    public IBossPhaseState Execute()
    {
        //throw new System.NotImplementedException();
        var state = _internalState.Execute();
        // switch internal state if needed
        if (state != null)
        {
            Debug.Log("Switching internal state 2");
            _internalState.Exit();
            _internalState = state;
            _internalState.Enter(_bossData);
        }
        return !Alive() ? null : new BossPhaseThree(); // TODO change to phase 3 instead of phase 1
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

        _bossData.PhasePlatforms[1].SetActive(false);
    }

    public bool Alive()
    {
        //throw new System.NotImplementedException();
        return _internalState.Alive();
    }

    // this will be called via monobehaviour
    public void TakeDamage(int value)
    {
        Debug.Log("Taking " + value + " damage 2");
        _internalState.TakeDamage(value);
    }
}
