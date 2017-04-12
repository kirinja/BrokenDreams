using System;
using UnityEngine;

public struct GroundState3D : ICharacterState3D
{
    private readonly Controller3D controller;
    private Transform platform;
    private Vector2 previousPlatformPosition;

    public GroundState3D(Controller3D controller)
    {
        if (controller == null)
        {
            throw new ArgumentNullException("controller");
        }

        this.controller = controller;
        platform = null;
        previousPlatformPosition = Vector2.zero;
    }

    public void Enter()
    {
        controller.Velocity = new Vector3(controller.Velocity.x, 0f);
    }

    public void Exit()
    {
    }

    public void Update(Vector2 input)
    {
        UpdateVelocity(input);
        ClampToPlatform();

        if (Input.GetButtonDown("Taunt"))
        {
            controller.Animator.SetTrigger("Wave");
        }
    }

    private void ClampToPlatform()
    {
        var gameObject = GetGround();
        if (gameObject)
        {
            if (gameObject.transform == platform)
            {
                controller.transform.position += platform.position - new Vector3(previousPlatformPosition.x, previousPlatformPosition.y);
                previousPlatformPosition = platform.position;
            }
            else
            {
                platform = gameObject.transform;
                previousPlatformPosition = platform.position;
            }
        }
        else
        {
            platform = null;
        }
    }

    public CharacterStateSwitch3D HandleCollisions(CollisionFlags collisionFlags)
    {
        CharacterStateSwitch3D stateSwitch;
        if ((collisionFlags & CollisionFlags.Below) == CollisionFlags.Below)
        {
            controller.Velocity = new Vector3(controller.Velocity.x, 0f);
            stateSwitch = new CharacterStateSwitch3D();
        }
        else
        {
            stateSwitch = new CharacterStateSwitch3D(new AirState3D(controller));
        }
        if ((collisionFlags & CollisionFlags.Sides) == CollisionFlags.Sides)
        {
            controller.Velocity = new Vector3(0f, controller.Velocity.y, 0f);
        }
        if ((collisionFlags & CollisionFlags.Above) == CollisionFlags.Above)
        {
            controller.Velocity = new Vector3(controller.Velocity.x, 0f);
        }

        return stateSwitch;
    }

    private void UpdateVelocity(Vector2 input)
    {
        var desiredVelocity = new Vector2(input.x * controller.GetComponent<PlayerAttributes>().MaxSpeed, 0f);

        UpdateRotation(input);
        ApplyAcceleration(desiredVelocity);
        ApplyFriction(desiredVelocity);
        ApplyGravity();
    }

    private void UpdateRotation(Vector2 input)
    {
        var currentAngle = controller.transform.eulerAngles.y;
        var desiredAngle = Mathf.Sign(controller.Forward.x) > 0f ? 90f : -90f;
        if (Mathf.Abs(input.x) > 0)
        {
            desiredAngle = Mathf.Sign(input.x) > 0f ? 90f : -90f;
            controller.Forward = new Vector2(input.x, 0f);
        }

        var rotationAngle = desiredAngle - currentAngle;
        if (rotationAngle > 180f)
        {
            rotationAngle = rotationAngle - 360f;
        }
        else if (rotationAngle < -180f)
        {
            rotationAngle = rotationAngle + 360;
        }
        var rotationSpeed = (180f / controller.GetComponent<PlayerAttributes>().MaxRotationTime) * Mathf.Sign(rotationAngle) * Time.deltaTime;

        if (Mathf.Abs(rotationSpeed) > Mathf.Abs(rotationAngle))
        {
            rotationSpeed = rotationAngle;
        }

        currentAngle += rotationSpeed;
        if (currentAngle > 360f)
        {
            currentAngle -= 360f;
        }

        controller.transform.eulerAngles = new Vector3(0f, currentAngle, 0f);
    }

    private void ApplyAcceleration(Vector2 desiredVelocity)
    {
        if (Mathf.Abs(desiredVelocity.x) <= 0) return;

        var attributes = controller.GetComponent<PlayerAttributes>();
        var acceleration = Mathf.Sign(controller.Forward.x) * Time.deltaTime * (attributes.MaxSpeed / attributes.GroundAccelerationTime);

        if (desiredVelocity.x > 0f && controller.Velocity.x + acceleration > desiredVelocity.x || desiredVelocity.x < 0f && controller.Velocity.x + acceleration < desiredVelocity.x)
        {
            acceleration = desiredVelocity.x - controller.Velocity.x;
        }
        controller.Velocity += Vector2.right * acceleration;
    }

    private void ApplyFriction(Vector2 desiredVelocity)
    {
        var attributes = controller.GetComponent<PlayerAttributes>();
        var deacceleration = attributes.MaxSpeed / attributes.GroundDeaccelerationTime;
        var frictionDirection = -Mathf.Sign(controller.Velocity.x);
        var friction = frictionDirection * deacceleration * Time.deltaTime;

        if ((int)Mathf.Sign(controller.Velocity.x) == (int)Mathf.Sign(desiredVelocity.x))
        {
            if (Mathf.Abs(controller.Velocity.x) > Mathf.Abs(desiredVelocity.x))
            {
                var newVelocity = controller.Velocity + Vector2.right * friction;

                if (Mathf.Abs(newVelocity.x) < Mathf.Abs(desiredVelocity.x))
                {
                    newVelocity = new Vector2(desiredVelocity.x, newVelocity.y);
                }
                if ((int) Mathf.Sign(newVelocity.x) != (int) Mathf.Sign(controller.Velocity.x))
                {
                    newVelocity = new Vector2(0f, newVelocity.y);
                }
                controller.Velocity = newVelocity;
            }
        }
        else
        {
            var newVelocity = controller.Velocity + Vector2.right * friction;
            
            if ((int)Mathf.Sign(newVelocity.x) != (int)Mathf.Sign(controller.Velocity.x))
            {
                newVelocity = new Vector2(0f, newVelocity.y);
            }
            controller.Velocity = newVelocity;
        }
    }

    private void ApplyGravity()
    {
        controller.Velocity += controller.Gravity * Time.deltaTime;
    }

    private GameObject GetGround()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(controller.transform.position, Vector3.down, out hitInfo))
        {
            return hitInfo.transform.gameObject;
        }
        return null;
    }

    public void AttemptStateSwitch(CharacterStateSwitch3D state)
    {
        if (state.NewState == null)
        {
            return;
        }

        if (state.NewState is AirState3D || state.NewState is DashState3D)
        {
            controller.ChangeCharacterState(state);
        }
    }
}