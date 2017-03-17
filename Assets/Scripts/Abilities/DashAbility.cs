using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Dash")]
class DashAbility : Ability
{
    private void OnEnable()
    {
        Color = (Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors).DashColor;
    }

    public override IPlayerState Use(bool is3D)
    {
        Debug.Log("Using Dash");
        timeLeft = Cooldown;
        return null;
    }
}