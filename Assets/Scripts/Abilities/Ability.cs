using System;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public float Cooldown;

    [NonSerialized]
    protected float timeLeft; 

    public Color Color { get; protected set; }
    public bool CanUse { get { return timeLeft <= 0; } }

    public abstract CharacterStateSwitch3D Use(Controller3D controller);

    private void Start()
    {
        timeLeft = 0f;
    }

    public virtual void UpdateAbility()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
        }
    }
}