using UnityEngine;


[CreateAssetMenu(menuName = "Abilities/Jump")]
public class JumpAbility : Ability
{
    private void OnEnable()
    {
        Color = (Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors).JumpColor;
    }


    public override CharacterStateSwitch3D Use(Controller3D controller)
    {
        timeLeft = Cooldown;

        return new CharacterStateSwitch3D(new AirState3D(controller, true));
    }
}