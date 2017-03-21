using System;
using UnityEngine;

public struct GroundState3D : ICharacterState3D
{
    private readonly Controller3D controller;
    private readonly Velocity3D velocity;

    public Velocity3D Velocity { get { return velocity; } }

    public GroundState3D(Controller3D controller, Velocity3D velocity)
    {
        if (controller == null)
        {
            throw new ArgumentNullException("controller");
        }
        if (velocity == null)
        {
            throw new ArgumentNullException("velocity");
        }

        this.controller = controller;
        this.velocity = velocity;
    }

    public void Enter()
    {
        velocity.SetY(0.0f);
    }

    public void Exit()
    {
    }

    public void Update(float horizontalInput, float verticalInput, float deltaTime)
    {
        UpdateVelocity(horizontalInput, verticalInput, deltaTime);
    }

    public CharacterStateSwitch3D HandleCollisions(CollisionFlags collisionFlags)
    {
        CharacterStateSwitch3D stateSwitch;
        if ((collisionFlags & CollisionFlags.Below) == CollisionFlags.Below)
        {
            stateSwitch = new CharacterStateSwitch3D();
        }
        else
        {
            stateSwitch = new CharacterStateSwitch3D(new AirState3D(controller, velocity));
        }

        return stateSwitch;
    }

    private void UpdateVelocity(float horizontalInput, float verticalInput, float deltatime)
    {
        var movementInput = new Vector3(horizontalInput, 0f, verticalInput);
        var smoothDampDataX = GetSmoothDampData(movementInput.x);
        var smoothDampDataZ = GetSmoothDampData(movementInput.z);

        velocity.SetY(-20.0f);
        velocity.SmoothDampUpdate(movementInput, smoothDampDataX, smoothDampDataZ, deltatime);
    }

    private SmoothDampData GetSmoothDampData(float input)
    {
        var targetVelocity = input * controller.Attributes.MaxSpeed;
        var smoothTime = controller.Attributes.GroundAccelerationTime;
        if (Mathf.Abs(input) < MathHelper.FloatEpsilon)
        {
            smoothTime *= controller.GroundDeaccelerationScale;
        }

        return new SmoothDampData(targetVelocity, smoothTime);
    }

    public void AttemptStateSwitch(CharacterStateSwitch3D state)
    {
        if (state.NewState == null)
        {
            return;
        }

        if (state.NewState is AirState3D)
        {
            controller.ChangeCharacterState(state);
        }
    }
}