using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Attackable
{
    public abstract void changeState(EnemyState e);
}
