using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Push")]
class PushAbility : Ability
{
    private void OnEnable()
    {
        Color = (Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors).PushColor;
    }

    public override IPlayerState Use(bool is3D)
    {
        Debug.Log("Using Push");
        timeLeft = Cooldown;
        return null;
    }
}