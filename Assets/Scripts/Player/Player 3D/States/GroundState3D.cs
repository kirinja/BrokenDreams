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

    public void Update(Vector2 input, bool forceRotate)
    {
        UpdateVelocity(input, forceRotate);
        var gameObject = GetGround();
        if (gameObject)
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

		if (Input.GetButtonDown("Taunt")){
			controller.animator.SetTrigger ("Wave");
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

    private void UpdateVelocity(Vector2 input, bool forceRotate)
    {
        var move = new Vector3(input.x, 0, input.y);
        move.Normalize();
        var currentAngle = controller.transform.eulerAngles.y;
        var inputAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
        var direction = controller.CameraTransform.eulerAngles.y + inputAngle;
        var forwardMovement = move.magnitude;
        var attributes = controller.GetComponent<PlayerAttributes>();
        var desiredForwardVelocity = forwardMovement * attributes.MaxSpeed;
        var acceleration = attributes.MaxSpeed / attributes.GroundAccelerationTime;
        var deacceleration = attributes.MaxSpeed / attributes.GroundDeaccelerationTime;

        var actualAngle = controller.transform.eulerAngles.y;
        controller.transform.eulerAngles = new Vector3(0, direction, 0);
        var desiredAngleVelocity = controller.transform.InverseTransformDirection(controller.Velocity);

        // Change character direction
        if (input.magnitude > float.Epsilon || forceRotate)
        {
            if (input.magnitude > float.Epsilon)
            {
                controller.InputForward = controller.transform.forward;
            }
            else
            {
                direction = Mathf.Atan2(controller.InputForward.x, controller.InputForward.z) * Mathf.Rad2Deg;
            }
            var rotationAngle = direction - currentAngle;
            if (rotationAngle > 180f)
            {
                rotationAngle = rotationAngle - 360f;
            }
            else if (rotationAngle < -180f)
            {
                rotationAngle = rotationAngle + 360;
            }
            var rotationSpeed = (180f / attributes.MaxRotationTime) * Mathf.Sign(rotationAngle) * Time.deltaTime;

            if (Mathf.Abs(rotationSpeed) > Math.Abs(rotationAngle))
            {
                rotationSpeed = rotationAngle;
            }

            var finalAngle = actualAngle + rotationSpeed;
            if (finalAngle > 360f)
            {
                finalAngle -= 360f;
            }

            actualAngle = finalAngle;
        }
        //var currentLocalVelocity = controller.transform.InverseTransformDirection(controller.Velocity);


        // Calculate and apply acceleration
        if (forwardMovement > float.Epsilon)
        {
            if (desiredAngleVelocity.z < desiredForwardVelocity)
            {
                var accelerationAmount = Time.deltaTime * acceleration;
                if (desiredAngleVelocity.z + accelerationAmount > desiredForwardVelocity)
                {
                    accelerationAmount = desiredForwardVelocity - desiredAngleVelocity.z;
                }
                desiredAngleVelocity.z += accelerationAmount;
            }
        }

        // Calculate and apply friction
        var invertedFrictionDirection = desiredAngleVelocity - new Vector3(0, 0, desiredForwardVelocity);
        if ((int)Mathf.Sign(desiredAngleVelocity.z) != (int)Mathf.Sign(invertedFrictionDirection.z))
        {
            invertedFrictionDirection.z = 0f;
        }
        var friction = -invertedFrictionDirection.normalized * deacceleration * Time.deltaTime;
        var newVelocity = desiredAngleVelocity + friction;
        if (desiredAngleVelocity.z > desiredForwardVelocity && desiredForwardVelocity > newVelocity.z)
        {
            newVelocity.z = desiredForwardVelocity;
        }
        if ((int)Mathf.Sign(newVelocity.x) != (int)Mathf.Sign(desiredAngleVelocity.x)) newVelocity.x = 0f;
        if ((int)Mathf.Sign(newVelocity.y) != (int)Mathf.Sign(desiredAngleVelocity.y)) newVelocity.y = 0f;
        if ((int)Mathf.Sign(newVelocity.z) != (int)Mathf.Sign(desiredAngleVelocity.z)) newVelocity.z = 0f;
        desiredAngleVelocity = newVelocity;

        var gravity = new Vector3(0f, (-2 * attributes.MaxJumpHeight * Mathf.Pow(attributes.MaxSpeed, 2)) / (Mathf.Pow(attributes.MaxJumpLength / 2, 2)), 0f);
        desiredAngleVelocity += gravity * Time.deltaTime;

        controller.Velocity = controller.transform.TransformDirection(desiredAngleVelocity);
        controller.transform.eulerAngles = new Vector3(0f, actualAngle, 0f);
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