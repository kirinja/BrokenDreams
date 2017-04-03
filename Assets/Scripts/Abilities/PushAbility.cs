﻿using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Push")]
class PushAbility : Ability
{
    public float PushRange = 0.5f;

    private void OnEnable()
    {
        Color = (Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors).PushColor;
    }

    public override CharacterStateSwitch3D Use(Controller3D controller)
    {
        timeLeft = Cooldown;

		controller.animator.SetTrigger ("Push");

		controller.transform.Find("Push").Find("SpeedLines").GetComponent<ParticleSystem>().Play();
		controller.transform.Find("Push").Find("Magic").GetComponent<ParticleSystem>().Play();

        RaycastHit hitInfo;
        if (Physics.Raycast(controller.transform.position,
            controller.InputForward, out hitInfo, PushRange + controller.GetComponent<Collider>().bounds.extents.z))
        {
            var pushable = hitInfo.transform.GetComponent<Pushable>();
            if (pushable)
            {
                pushable.Push(controller.InputForward);
            }
        }

        return new CharacterStateSwitch3D();
    }
}