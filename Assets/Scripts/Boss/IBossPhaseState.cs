using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBossPhaseState
{
    void Enter(BossBehaviour data);
    IBossPhaseState Execute();
    void Exit();

    bool Alive();
    void TakeDamage(int value);
}
