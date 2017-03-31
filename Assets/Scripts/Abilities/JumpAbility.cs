using System;
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
        Debug.Log("Using Jump");
        timeLeft = Cooldown;

		controller.transform.Find("Jump").Find("JumpTrail01").GetComponent<ParticleSystem>().Play();
		controller.transform.Find("Jump").Find("JumpTrail02").GetComponent<ParticleSystem>().Play();
		controller.transform.Find("Jump").Find("JumpTrail03").GetComponent<ParticleSystem>().Play();

        return new CharacterStateSwitch3D(new AirState3D(controller, true));
    }
}