using UnityEngine;

public abstract class Attackable : MonoBehaviour
{
    public abstract void Damage(int damage = 1);
}
