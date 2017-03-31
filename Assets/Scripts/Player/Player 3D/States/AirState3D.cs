using System;
using UnityEngine;

public struct AirState3D : ICharacterState3D
{
    private readonly Controller3D controller;
    private bool willJump, jumping;

    public AirState3D(Controller3D controller, bool willJump = false)
    {
        if (controller == null)
        {
            throw new ArgumentNullException("controller");
        }

        jumping = false;
        this.willJump = willJump;
        this.controller = controller;
    }

    public void Enter()
    {
		controller.animator.SetBool("InAir", true);
    }

    public void Exit()
    {
		controller.animator.SetBool("InAir", false);
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
        var currentAngle = controller.transform.eulerAngles.y;
        var inputAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
        var direction = controller.CameraTransform.eulerAngles.y + inputAngle;
        var forwardMovement = move.magnitude;
        var attributes = controller.GetComponent<PlayerAttributes>();
        var desiredForwardVelocity = forwardMovement * attributes.MaxSpeed;
        var acceleration = attributes.MaxSpeed / attributes.AirAccelerationTime;

        var actualAngle = controller.transform.eulerAngles.y;
        controller.transform.eulerAngles = new Vector3(0, direction, 0);
        var desiredAngleVelocity = controller.transform.InverseTransformDirection(controller.Velocity);

        // Change character direction
        if (input.magnitude > float.Epsilon)
        {
            controller.InputForward = controller.transform.forward;
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
                desiredAngleVelocity.z += accelerationAmount; // Apply acceleration
            }
        }

        if (willJump)
        {
            desiredAngleVelocity.y = ((2 * attributes.MaxJumpHeight * attributes.MaxSpeed) / (attributes.MaxJumpLength / 2));
            willJump = false;
            jumping = true;

			controller.transform.Find("Jump").Find("JumpTrail01").GetComponent<ParticleSystem>().Play();
			controller.transform.Find("Jump").Find("JumpTrail02").GetComponent<ParticleSystem>().Play();
			controller.transform.Find("Jump").Find("JumpTrail03").GetComponent<ParticleSystem>().Play();
        }

        var gravity = new Vector3(0f, (-2 * attributes.MaxJumpHeight * Mathf.Pow(attributes.MaxSpeed, 2)) / (Mathf.Pow(attributes.MaxJumpLength / 2, 2)), 0f);
        desiredAngleVelocity += gravity * Time.deltaTime;

        var minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity.y) * attributes.MinJumpHeight);
        if (Input.GetButtonUp("Use Ability") && jumping && desiredAngleVelocity.y > minJumpVelocity)
        {
            Debug.Log("short hop");
            desiredAngleVelocity = new Vector3(0f, minJumpVelocity, 0f);
            jumping = false;
        }

        controller.Velocity = controller.transform.TransformDirection(desiredAngleVelocity);
        controller.transform.eulerAngles = new Vector3(0f, actualAngle, 0f);

        if (controller.Velocity.y <= 0f)
        {
            jumping = false;
        }

		if (!jumping && controller.transform.Find("Jump").Find("JumpTrail01").GetComponent<ParticleSystem>().isPlaying) {
			controller.transform.Find("Jump").Find("JumpTrail01").GetComponent<ParticleSystem>().Stop();
			controller.transform.Find("Jump").Find("JumpTrail02").GetComponent<ParticleSystem>().Stop();
			controller.transform.Find("Jump").Find("JumpTrail03").GetComponent<ParticleSystem>().Stop();
		}
    }

    public void AttemptStateSwitch(CharacterStateSwitch3D state)
    {
        if (state.NewState is GroundState3D || state.NewState is DashState3D)
        {
            controller.ChangeCharacterState(state);
        }
    }
}