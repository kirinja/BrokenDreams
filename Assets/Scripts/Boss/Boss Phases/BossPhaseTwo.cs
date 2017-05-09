using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseTwo : IBossPhaseState
{
    private IBossSubState _internalState;
    private BossBehaviour _bossData;

    // check the amount of enemies here, if it is below a certain value then trigger the "attack" state, stay in this state until we damage the head, then go into defend again

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        _bossData.PhasePlatforms[1].SetActive(true);
        _bossData.BossPhase2.SetActive(true);
        _bossData.Phase2Launch.SetActive(true);
        _internalState = new BossSubTwoDefend();
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
        return !Alive() ? null : new BossPhaseThree();
    }

    public void Exit()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject g in enemies)
        {
            Object.Destroy(g);
        }

        _bossData.PhasePlatforms[1].SetActive(false);
        _bossData.BossPhase2.SetActive(false);
        _bossData.Phase2Launch.SetActive(false);

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
