using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Attack")]
class AttackAbility : Ability
{
    private void OnEnable()
    {
        Color = (Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors).AttackColor;
    }

    public override IPlayerState Use(bool is3D)
    {
        Debug.Log("Using Attack");
        timeLeft = Cooldown;
        return null;
    }
}