using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour {


    public abstract void Damage();
    public abstract void changeState(EnemyState e);

}
