using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBossSubState
{
    void Enter(BossBehaviour data);
    IBossSubState Execute();
    void Exit();

    bool Alive();

    void TakeDamage(int value);
}
