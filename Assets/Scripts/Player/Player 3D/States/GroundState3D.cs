using System;
using UnityEngine;

public struct GroundState3D : ICharacterState3D
{
    private readonly Controller3D controller;
    private Transform platform;
    private Vector3 previousPlatformPosition;

    public GroundState3D(Controller3D controller)
    {
        if (controller == null)
        {
            throw new ArgumentNullException("controller");
        }

        this.controller = controller;
        platform = null;
        previousPlatformPosition = Vector3.zero;
    }

    public void Enter()
    {
        controller.Velocity = new Vector3(controller.Velocity.x, 0f, controller.Velocity.z);
    }

    public void Exit()
    {
    }

    public void Update(Vector2 input)
    {
        UpdateVelocity(input);
        var gameObject = GetGround();
        if (gameObject && gameObject.CompareTag("Enemy"))
        {
            if (gameObject.transform == platform)
            {
                controller.transform.position += platform.position - previousPlatformPosition;
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
            controller.Velocity = new Vector3(controller.Velocity.x, 0f, controller.Velocity.z);
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
        var acceleration = attributes.MaxSpeed / attributes.GroundAccelerationTime;
        var deacceleration = attributes.MaxSpeed / attributes.GroundDeaccelerationTime;

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
                currentLocalVelocity.z += accelerationAmount;
            }
        }

        // Calculate and apply friction
        var invertedFrictionDirection = currentLocalVelocity - new Vector3(0, 0, desiredForwardVelocity);
        if ((int)Mathf.Sign(currentLocalVelocity.z) != (int)Mathf.Sign(invertedFrictionDirection.z))
        {
            invertedFrictionDirection.z = 0f;
        }
        var friction = -invertedFrictionDirection.normalized * deacceleration * Time.deltaTime;
        var newVelocity = currentLocalVelocity + friction;
        if (currentLocalVelocity.z > desiredForwardVelocity && desiredForwardVelocity > newVelocity.z)
        {
            newVelocity.z = desiredForwardVelocity;
        }
        if ((int)Mathf.Sign(newVelocity.x) != (int)Mathf.Sign(currentLocalVelocity.x)) newVelocity.x = 0f;
        if ((int)Mathf.Sign(newVelocity.y) != (int)Mathf.Sign(currentLocalVelocity.y)) newVelocity.y = 0f;
        if ((int)Mathf.Sign(newVelocity.z) != (int)Mathf.Sign(currentLocalVelocity.z)) newVelocity.z = 0f;
        currentLocalVelocity = newVelocity;

        var gravity = new Vector3(0f, (-2 * attributes.MaxJumpHeight * Mathf.Pow(attributes.MaxSpeed, 2)) / (Mathf.Pow(attributes.MaxJumpLength / 2, 2)), 0f);
        currentLocalVelocity += gravity * Time.deltaTime;

        controller.Velocity = controller.transform.TransformDirection(currentLocalVelocity);
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