using System;
using UnityEngine;

public struct AirState3D : ICharacterState3D
{
    private readonly Controller3D controller;
    private bool jump;

    public AirState3D(Controller3D controller, bool jump = false)
    {
        if (controller == null)
        {
            throw new ArgumentNullException("controller");
        }

        this.jump = jump;
        this.controller = controller;
    }

    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public void Update(Vector2 input)
    {
        UpdateVelocity(input);
    }

    public CharacterStateSwitch3D HandleCollisions(CollisionFlags collisionFlags)
    {
        CharacterStateSwitch3D stateSwitch;
        if ((collisionFlags & CollisionFlags.Below) == CollisionFlags.Below)
        {
            if (controller.IsTraversableSlope(controller.ColliderHeight * 10.0f))
            {
                stateSwitch = new CharacterStateSwitch3D(new GroundState3D(controller));
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
        if ((collisionFlags & CollisionFlags.Sides) == CollisionFlags.Sides)
        {
            controller.Velocity = new Vector3(0f, controller.Velocity.y, 0f);
        }
        if ((collisionFlags & CollisionFlags.Above) == CollisionFlags.Above && controller.Velocity.y > 0f)
        {
            controller.Velocity = new Vector3(controller.Velocity.x, 0f, controller.Velocity.z);
        }

        return stateSwitch;
    }

    private void UpdateVelocity(Vector2 input)
    {
        var move = new Vector3(input.x, 0, input.y);
        move.Normalize();
        //var currentAngle = controller.transform.eulerAngles.y;
        var inputAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
        var direction = controller.CameraTransform.eulerAngles.y + inputAngle;
        var forwardMovement = move.magnitude;
        var attributes = controller.GetComponent<PlayerAttributes>();
        var desiredForwardVelocity = forwardMovement * attributes.MaxSpeed;
        var acceleration = attributes.MaxSpeed / attributes.AirAccelerationTime;

        // Change character direction
        if (input.magnitude > float.Epsilon)
        {
            //var rotationAngle = Mathf.Abs(direction - currentAngle);
            //var rotationTime = attributes.MaxRotationTime / 180f * rotationAngle;
            //var rotationSpeed = rotationAngle / rotationTime;
            //direction = Mathf.LerpAngle(currentAngle, direction, Mathf.Min(1f, Time.deltaTime * rotationSpeed));
            controller.transform.eulerAngles = new Vector3(0, direction, 0);
        }
        var currentLocalVelocity = controller.transform.InverseTransformDirection(controller.Velocity);

        // Calculate and apply acceleration
        if (forwardMovement > float.Epsilon)
        {
            if (currentLocalVelocity.z < desiredForwardVelocity)
            {
                var accelerationAmount = Time.deltaTime * acceleration;
                if (currentLocalVelocity.z + accelerationAmount > desiredForwardVelocity)
                {
                    accelerationAmount = desiredForwardVelocity - currentLocalVelocity.z;
                }
                currentLocalVelocity.z += accelerationAmount; // Apply acceleration
            }
        }

        if (jump)
        {
            currentLocalVelocity.y = ((2 * attributes.MaxJumpHeight * attributes.MaxSpeed) / (attributes.MaxJumpLength / 2));
            jump = false;
        }

        var gravity = new Vector3(0f, (-2 * attributes.MaxJumpHeight * Mathf.Pow(attributes.MaxSpeed, 2)) / (Mathf.Pow(attributes.MaxJumpLength / 2, 2)), 0f);
        currentLocalVelocity += gravity * Time.deltaTime;

        controller.Velocity = controller.transform.TransformDirection(currentLocalVelocity);
    }

    public void AttemptStateSwitch(CharacterStateSwitch3D state)
    {
        if (state.NewState is GroundState3D || state.NewState is DashState3D)
        {
            controller.ChangeCharacterState(state);
        }
    }
}