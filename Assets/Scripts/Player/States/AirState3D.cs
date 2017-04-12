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
		controller.Animator.SetBool("InAir", true);
    }

    public void Exit()
    {
		controller.Animator.SetBool("InAir", false);
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
            controller.Velocity = new Vector2(0f, controller.Velocity.y);
        }
        if ((collisionFlags & CollisionFlags.Above) == CollisionFlags.Above && controller.Velocity.y > 0f)
        {
            controller.Velocity = new Vector2(controller.Velocity.x, 0f);
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
        CheckJump();
        HandleJumpEnded();
		HandleJumpParticles();
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
        var acceleration = Mathf.Sign(controller.Forward.x) * Time.deltaTime * (attributes.MaxSpeed / attributes.AirAccelerationTime);

        if (desiredVelocity.x > 0f && controller.Velocity.x + acceleration > desiredVelocity.x || desiredVelocity.x < 0f && controller.Velocity.x + acceleration < desiredVelocity.x)
        {
            acceleration = desiredVelocity.x - controller.Velocity.x;
        }
        controller.Velocity += Vector2.right * acceleration;
    }

    private void ApplyFriction(Vector2 desiredVelocity)
    {
        var attributes = controller.GetComponent<PlayerAttributes>();
        var deacceleration = attributes.MaxSpeed / attributes.AirDeaccelerationTime;
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
                if ((int)Mathf.Sign(newVelocity.x) != (int)Mathf.Sign(controller.Velocity.x))
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

    private void HandleJumpParticles()
    {
        if (!jumping && controller.transform.Find("Jump").Find("JumpTrail01").GetComponent<ParticleSystem>().isPlaying)
        {
            controller.transform.Find("Jump").Find("JumpTrail01").GetComponent<ParticleSystem>().Stop();
            controller.transform.Find("Jump").Find("JumpTrail02").GetComponent<ParticleSystem>().Stop();
            controller.transform.Find("Jump").Find("JumpTrail03").GetComponent<ParticleSystem>().Stop();
        }
    }

    private void CheckJump()
    {
        if (willJump)
        {
            controller.Velocity = new Vector2(controller.Velocity.x, controller.MaxJumpVelocity);
            willJump = false;
            jumping = true;

            controller.transform.Find("Jump").Find("JumpTrail01").GetComponent<ParticleSystem>().Play();
            controller.transform.Find("Jump").Find("JumpTrail02").GetComponent<ParticleSystem>().Play();
            controller.transform.Find("Jump").Find("JumpTrail03").GetComponent<ParticleSystem>().Play();
        }
    }

    private void HandleJumpEnded()
    {
        var minJumpVelocity = controller.MinJumpVelocity;
        if (Input.GetButtonUp("Use Ability") && jumping && controller.Velocity.y > minJumpVelocity)
        {
            controller.Velocity = new Vector2(controller.Velocity.x, minJumpVelocity);
            jumping = false;
        }

        if (controller.Velocity.y <= 0f)
        {
            jumping = false;
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