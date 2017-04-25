using UnityEngine;

[RequireComponent(typeof(AttackableMaterialChanger))]
public abstract class Attackable : MonoBehaviour
{
    public abstract void Damage(int damage = 1);
}
