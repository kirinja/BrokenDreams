using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Push")]
class PushAbility : Ability
{
    private void OnEnable()
    {
        Color = (Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors).PushColor;
    }

    public override CharacterStateSwitch3D Use(Controller3D controller)
    {
        Debug.Log("Using Push");
        timeLeft = Cooldown;
        return new CharacterStateSwitch3D();
    }
}