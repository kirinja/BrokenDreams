using System;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public float Cooldown;

    [NonSerialized]
    protected float timeLeft; 

    public Color Color { get; protected set; }
    public bool CanUse { get { return timeLeft <= 0; } }

    public abstract IPlayerState Use(bool is3D);

    private void Start()
    {
        timeLeft = 0f;
    }

    public void UpdateTime()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
        }
    }
}