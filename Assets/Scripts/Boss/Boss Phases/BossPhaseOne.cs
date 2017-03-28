using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseOne : IBossPhaseState
{
    private IBossSubState _internalState;
    private BossBehaviour _bossData;

    public void Enter(BossBehaviour data)
    {
        Debug.Log("Boss Phase 1");
        //throw new System.NotImplementedException();
        _bossData = data;
        _bossData.PhasePlatforms[0].SetActive(true);
        _bossData.BossPhase1.SetActive(true);
        _internalState = new BossSubOneIdle();
        _internalState.Enter(_bossData); // TODO
    }

    public IBossPhaseState Execute()
    {
        
        //throw new System.NotImplementedException();
        var state = _internalState.Execute();
        // switch internal state if needed
        if (state != null)
        {
            Debug.Log("Switching internal state 1");
            _internalState.Exit();
            _internalState = state;
            _internalState.Enter(_bossData); // TODO
        }
        return !Alive() ? null : new BossPhaseTwo(); // TODO change to phase 2 instead of phase 1
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

        _bossData.PhasePlatforms[0].SetActive(false);
        _bossData.BossPhase1.SetActive(false);
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
    }

    public bool Alive()
    {
        //throw new System.NotImplementedException();
        return _internalState.Alive();
    }

    // this will be called via monobehaviour
    public void TakeDamage(int value)
    {
        Debug.Log("Taking " + value + " damage");
        _internalState.TakeDamage(value);
    }
}
