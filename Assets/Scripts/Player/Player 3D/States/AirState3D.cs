using System;
using UnityEngine;

public struct AirState3D : ICharacterState3D
{
    private readonly Controller3D controller;
    private readonly Velocity3D velocity;
    private bool jump;

    public AirState3D(Controller3D controller, Velocity3D velocity, bool jump = false)
    {
        if (controller == null)
        {
            throw new ArgumentNullException("controller");
        }
        if (velocity == null)
        {
            throw new ArgumentNullException("velocity");
        }

        this.jump = jump;
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

    public void Update(float horizontalInput, float verticalInput, float deltatime)
    {
        if (jump)
        {
            PerformJump();
        }
        UpdateVelocity(horizontalInput, verticalInput, deltatime);
    }

    public CharacterStateSwitch3D HandleCollisions(CollisionFlags collisionFlags)
    {
        CharacterStateSwitch3D stateSwitch;
        if ((collisionFlags & CollisionFlags.Below) == CollisionFlags.Below)
        {
            if (controller.IsTraversableSlope(controller.ColliderHeight * 10.0f))
            {
                stateSwitch = new CharacterStateSwitch3D(new GroundState3D(controller, velocity));
            }
            else
            {
                stateSwitch = new CharacterStateSwitch3D();
            }
        }
        else
        {
            stateSwitch = new CharacterStateSwitch3D();
        }

        return stateSwitch;
    }

    private void PerformJump()
    {
        velocity.SetY(controller.MaxJumpVelocity);
        jump = false;
    }

    private void UpdateVelocity(float horizontalInput, float verticalInput, float deltaTime)
    {
        // TODO: Change to use my own controller thing
        var movementInput = new Vector3(horizontalInput, 0f , verticalInput);
        var smoothDampDataX = GetSmoothDampData(movementInput.x);
        var smoothDampDataZ = GetSmoothDampData(movementInput.z);

        velocity.AddY(controller.Gravity * deltaTime);
        velocity.SmoothDampUpdate(movementInput, smoothDampDataX, smoothDampDataZ, deltaTime);
    }

    private SmoothDampData GetSmoothDampData(float input)
    {
        var targetVelocity = input * controller.Attributes.MaxSpeed;
        var smoothTime = controller.Attributes.AirAccelerationTime;
        if (Mathf.Abs(input) < MathHelper.FloatEpsilon)
        {
            smoothTime *= controller.AirDeaccelerationScale;
        }

        return new SmoothDampData(targetVelocity, smoothTime);
    }

    public void AttemptStateSwitch(CharacterStateSwitch3D state)
    {
        if (state.NewState is GroundState3D)
        {
            controller.ChangeCharacterState(state);
        }
    }

    public Velocity3D Velocity { get { return velocity; } }
}