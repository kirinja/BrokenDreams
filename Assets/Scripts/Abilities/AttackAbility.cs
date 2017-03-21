using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Attack")]
class AttackAbility : Ability
{
    private void OnEnable()
    {
        Color = (Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors).AttackColor;
    }

    public override CharacterStateSwitch3D Use(Controller3D controller)
    {
        Debug.Log("Using Attack");
        timeLeft = Cooldown;
        return new CharacterStateSwitch3D();
    }
}