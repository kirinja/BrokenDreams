using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Dash")]
class DashAbility : Ability
{
    private void OnEnable()
    {
        Color = (Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors).DashColor;
    }

    public override CharacterStateSwitch3D Use(Controller3D controller)
    {
        timeLeft = Cooldown;

        controller.Animator.SetTrigger("Dash");

        return new CharacterStateSwitch3D(new DashState3D(controller));
    }
}