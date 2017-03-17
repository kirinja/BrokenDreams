using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Enemy01State {

    void Enter();
    void Exit();
    void Update();
    void Collision();
}
