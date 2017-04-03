using System;
using UnityEngine;

class DashState3D : ICharacterState3D
{
    private readonly Controller3D controller;
    private readonly Vector3 previousVelocity;
    private float timePassed;

    public DashState3D(Controller3D controller)
    {
        this.controller = controller;
        previousVelocity = controller.transform.InverseTransformDirection(controller.Velocity);
        var move = new Vector3(controller.MovementInput.x, 0, controller.MovementInput.y);
        move.Normalize();
        var inputAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
        var direction = controller.CameraTransform.eulerAngles.y + inputAngle;
        var attributes = controller.GetComponent<PlayerAttributes>();

        // Change character direction
        if (controller.MovementInput.magnitude > float.Epsilon)
        {
            controller.transform.eulerAngles = new Vector3(0, direction, 0);
        }

        var speed = attributes.DashLength / attributes.DashTime;
        var currentLocalVelocity = new Vector3(0, 0, speed);

        controller.Velocity = controller.transform.TransformDirection(currentLocalVelocity);
    }

    public void AttemptStateSwitch(CharacterStateSwitch3D state)
    {
    }

    public void Enter()
    {
        Debug.Log("Blerg");
        timePassed = 0;

		controller.transform.Find("Dash").Find("DashTrail").GetComponent<ParticleSystem>().Play();
		controller.transform.Find("Dash").Find("Sparkle").GetComponent<ParticleSystem>().Play();
    }

    public void Exit()
    {
        if (previousVelocity.z <= controller.Attributes.MaxSpeed)
        {
            controller.Velocity = controller.transform.TransformDirection(new Vector3(0f, 0f, previousVelocity.z));
        }
        else
        {
            controller.Velocity =
                controller.transform.TransformDirection(new Vector3(0f, 0f, controller.Attributes.MaxSpeed));
        }

		controller.transform.Find("Dash").Find("DashTrail").GetComponent<ParticleSystem>().Stop();
		controller.transform.Find("Dash").Find("Sparkle").GetComponent<ParticleSystem>().Stop();
    }

    public CharacterStateSwitch3D HandleCollisions(CollisionFlags collisionFlags)
    {
        if (collisionFlags == CollisionFlags.Sides)
        {
            controller.Velocity =
                controller.transform.TransformDirection(new Vector3(0f, 0f,
                    controller.GetComponent<PlayerAttributes>().MaxSpeed));

            return new CharacterStateSwitch3D(new AirState3D(controller));
        }
        return new CharacterStateSwitch3D();
    }

    public void Update(Vector2 input, bool forceRotate)
    {
        timePassed += Time.deltaTime;
        if (timePassed >= controller.GetComponent<PlayerAttributes>().DashTime || Input.GetButtonUp("Use Ability"))
        {
            controller.ChangeCharacterState(new CharacterStateSwitch3D(new AirState3D(controller)));
        }
    }
}