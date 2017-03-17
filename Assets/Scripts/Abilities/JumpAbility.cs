using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Jump")]
public class JumpAbility : Ability
{
    private void OnEnable()
    {
        Color = (Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors).JumpColor;
    }

    public override IPlayerState Use(bool is3D)
    {
        Debug.Log("Using Jump");
        timeLeft = Cooldown;
        return is3D ? new AirState3D(true) : null; // TODO: Maybe make it work for 2D
    }
}