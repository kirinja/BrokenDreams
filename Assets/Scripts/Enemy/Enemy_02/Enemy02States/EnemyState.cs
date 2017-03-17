using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EnemyState{

    void Enter();
    void Exit();
    void Update();
    bool getCanShoot();
}
