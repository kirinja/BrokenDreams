using UnityEngine;

class DashState3D : ICharacterState3D
{
    private readonly Controller3D controller;
    private readonly Vector2 previousVelocity;
    private Timer dashTimer;

    public DashState3D(Controller3D controller)
    {
        this.controller = controller;
        previousVelocity = controller.Velocity;

        dashTimer = new Timer(controller.GetComponent<PlayerAttributes>().DashTime);
        UpdateRotation();

        var attributes = controller.GetComponent<PlayerAttributes>();
        var speed = attributes.DashLength / attributes.DashTime;

        controller.Velocity = Mathf.Sign(controller.Forward.x) * speed * Vector2.right;
        Debug.Log(controller.Velocity);
    }

    private void UpdateRotation()
    {
        var desiredAngle = Mathf.Sign(controller.Forward.x) > 0f ? 90f : -90f;
        controller.transform.eulerAngles = new Vector3(0f, desiredAngle, 0f);
    }

    public bool AttemptStateSwitch(CharacterStateSwitch3D state)
    {
        return false;
    }

    public void Enter()
    {
        dashTimer.Reset();

		controller.transform.Find("Dash").Find("DashTrail").GetComponent<ParticleSystem>().Play();
		controller.transform.Find("Dash").Find("Sparkle").GetComponent<ParticleSystem>().Play();
    }

    public void Exit()
    {
        controller.Velocity = Mathf.Abs(previousVelocity.x) <= controller.Attributes.MaxSpeed ? new Vector2(previousVelocity.x, 0f) : new Vector2(Mathf.Sign(previousVelocity.x) * controller.GetComponent<PlayerAttributes>().MaxSpeed, 0f);

		controller.transform.Find("Dash").Find("DashTrail").GetComponent<ParticleSystem>().Stop();
		controller.transform.Find("Dash").Find("Sparkle").GetComponent<ParticleSystem>().Stop();
    }

    public CharacterStateSwitch3D HandleCollisions(CollisionFlags collisionFlags)
    {
        if (collisionFlags == CollisionFlags.Sides)
        {
            controller.Velocity = new Vector2(controller.GetComponent<PlayerAttributes>().MaxSpeed, 0f);

            return new CharacterStateSwitch3D(new AirState3D(controller));
        }
        return new CharacterStateSwitch3D();
    }

    public void Update(Vector2 input)
    {
        if (dashTimer.Update(Time.deltaTime) || Input.GetButtonUp("Use Ability 4"))
        {
            controller.ChangeCharacterState(new CharacterStateSwitch3D(new AirState3D(controller)));
        }
    }
}