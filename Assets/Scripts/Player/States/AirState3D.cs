using System;
using UnityEngine;

// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

public struct AirState3D : ICharacterState3D
{
    private readonly Controller3D _controller;
    private bool _willJump, _jumping, _jumpButtonUp;


    public AirState3D(Controller3D controller, bool willJump = false)
    {
        if (controller == null)
            throw new ArgumentNullException("controller");

        _jumpButtonUp = false;
        _jumping = false;
        _willJump = willJump;
        _controller = controller;
    }


    public void Enter()
    {
        _controller.Animator.SetBool("InAir", true);
    }


    public void Update()
    {
        _jumpButtonUp = Input.GetButtonUp("Use Ability 2");

        var desiredVelocity = new Vector2(
            _controller.MovementInput.x * _controller.GetComponent<PlayerAttributes>().MaxSpeed, 0f);

        UpdateRotation();
        ApplyAcceleration(desiredVelocity);
        ApplyFriction(desiredVelocity);
        ApplyGravity();
        CheckJump();
        HandleJumpEnded();
        HandleJumpParticles();
    }


    public void Exit()
    {
        _controller.Animator.SetBool("InAir", false);
    }


    public void LateUpdate()
    {
    }


    public void FixedUpdate()
    {
    }


    public CharacterStateSwitch3D HandleCollisions(CollisionFlags collisionFlags)
    {
        var stateSwitch = new CharacterStateSwitch3D();
        if ((collisionFlags & CollisionFlags.Below) == CollisionFlags.Below)
        {
            if (_controller.IsTraversableSlope(_controller.ColliderHeight * 10.0f))
            {
                // Collision sounds
                if (!_controller.CollideDown)
                {
                    _controller.GetComponentInChildren<TriggerSound>()
                        .PlayCollisionSound(Mathf.Min(1f,
                            Mathf.Abs(_controller.Velocity.y / _controller.Attributes.MaxSpeed)));
                }
                _controller.CollideDown = true;

                stateSwitch = new CharacterStateSwitch3D(new GroundState3D(_controller));
            }
            else
            {
                _controller.CollideDown = false;
            }
        }

        if ((collisionFlags & CollisionFlags.Sides) == CollisionFlags.Sides)
        {
            // Collision sounds
            if (!_controller.CollideSide)
            {
                _controller.GetComponentInChildren<TriggerSound>()
                    .PlayCollisionSound(Mathf.Min(1f,
                        Mathf.Abs(_controller.Velocity.x / _controller.Attributes.MaxSpeed)));
            }
            _controller.CollideSide = true;

            _controller.Velocity = new Vector2(0f, _controller.Velocity.y);
        }
        else
        {
            _controller.CollideSide = false;
        }

        if ((collisionFlags & CollisionFlags.Above) == CollisionFlags.Above && _controller.Velocity.y > 0f)
        {
            // Collision sounds
            if (!_controller.CollideDown)
            {
                _controller.GetComponentInChildren<TriggerSound>()
                    .PlayCollisionSound(Mathf.Min(1f,
                        Mathf.Abs(_controller.Velocity.y / _controller.Attributes.MaxSpeed)));
            }
            _controller.CollideDown = true;

            _controller.Velocity = new Vector2(_controller.Velocity.x, 0f);
        }
        else
        {
            _controller.CollideDown = false;
        }

        return stateSwitch;
    }


    private void UpdateRotation()
    {
        var currentAngle = _controller.transform.eulerAngles.y;
        var desiredAngle = Mathf.Sign(_controller.Forward.x) > 0f ? 90f : -90f;
        if (Mathf.Abs(_controller.MovementInput.x) > 0)
        {
            desiredAngle = Mathf.Sign(_controller.MovementInput.x) > 0f ? 90f : -90f;
            _controller.Forward = new Vector2(_controller.MovementInput.x, 0f).normalized;
        }

        var rotationAngle = desiredAngle - currentAngle;
        if (rotationAngle > 180f)
            rotationAngle = rotationAngle - 360f;
        else if (rotationAngle < -180f)
            rotationAngle = rotationAngle + 360;
        var rotationSpeed = 180f / _controller.GetComponent<PlayerAttributes>().MaxRotationTime *
                            Mathf.Sign(rotationAngle) * Time.deltaTime;

        if (Mathf.Abs(rotationSpeed) > Mathf.Abs(rotationAngle))
            rotationSpeed = rotationAngle;

        currentAngle += rotationSpeed;
        if (currentAngle > 360f)
            currentAngle -= 360f;

        _controller.transform.eulerAngles = new Vector3(0f, currentAngle, 0f);
    }


    private void ApplyAcceleration(Vector2 desiredVelocity)
    {
        if (Mathf.Abs(desiredVelocity.x) <= 0) return;

        var attributes = _controller.GetComponent<PlayerAttributes>();
        var acceleration = Mathf.Sign(_controller.Forward.x) * Time.deltaTime *
                           (attributes.MaxSpeed / attributes.AirAccelerationTime);

        if (desiredVelocity.x > 0f && _controller.Velocity.x + acceleration > desiredVelocity.x ||
            desiredVelocity.x < 0f && _controller.Velocity.x + acceleration < desiredVelocity.x)
            acceleration = desiredVelocity.x - _controller.Velocity.x;
        _controller.Velocity += Vector2.right * acceleration;
    }


    private void ApplyFriction(Vector2 desiredVelocity)
    {
        var attributes = _controller.GetComponent<PlayerAttributes>();
        var deacceleration = attributes.MaxSpeed / attributes.AirDeaccelerationTime;
        var frictionDirection = -Mathf.Sign(_controller.Velocity.x);
        var friction = frictionDirection * deacceleration * Time.deltaTime;

        if ((int) Mathf.Sign(_controller.Velocity.x) == (int) Mathf.Sign(desiredVelocity.x))
        {
            if (Mathf.Abs(_controller.Velocity.x) > Mathf.Abs(desiredVelocity.x))
            {
                var newVelocity = _controller.Velocity + Vector2.right * friction;

                if (Mathf.Abs(newVelocity.x) < Mathf.Abs(desiredVelocity.x))
                    newVelocity = new Vector2(desiredVelocity.x, newVelocity.y);
                if ((int) Mathf.Sign(newVelocity.x) != (int) Mathf.Sign(_controller.Velocity.x))
                    newVelocity = new Vector2(0f, newVelocity.y);
                _controller.Velocity = newVelocity;
            }
        }
        else
        {
            var newVelocity = _controller.Velocity + Vector2.right * friction;

            if ((int) Mathf.Sign(newVelocity.x) != (int) Mathf.Sign(_controller.Velocity.x))
                newVelocity = new Vector2(0f, newVelocity.y);
            _controller.Velocity = newVelocity;
        }
    }


    private void ApplyGravity()
    {
        _controller.Velocity += _controller.Gravity * Time.deltaTime;
    }


    private void HandleJumpParticles()
    {
        if (!_jumping && _controller.transform.Find("Jump")
                .Find("JumpTrail01")
                .GetComponent<ParticleSystem>()
                .isPlaying)
        {
            _controller.transform.Find("Jump").Find("JumpTrail01").GetComponent<ParticleSystem>().Stop();
            _controller.transform.Find("Jump").Find("JumpTrail02").GetComponent<ParticleSystem>().Stop();
            _controller.transform.Find("Jump").Find("JumpTrail03").GetComponent<ParticleSystem>().Stop();
        }
    }


    private void CheckJump()
    {
        if (_willJump)
        {
            _controller.Velocity = new Vector2(_controller.Velocity.x, _controller.MaxJumpVelocity);
            _willJump = false;
            _jumping = true;

            _controller.transform.Find("Jump").Find("JumpTrail01").GetComponent<ParticleSystem>().Play();
            _controller.transform.Find("Jump").Find("JumpTrail02").GetComponent<ParticleSystem>().Play();
            _controller.transform.Find("Jump").Find("JumpTrail03").GetComponent<ParticleSystem>().Play();
        }
    }


    private void HandleJumpEnded()
    {
        var minJumpVelocity = _controller.MinJumpVelocity;
        if (_jumpButtonUp && _jumping && _controller.Velocity.y > minJumpVelocity)
        {
            _controller.Velocity = new Vector2(_controller.Velocity.x, minJumpVelocity);
            _jumping = false;
        }

        if (_controller.Velocity.y <= 0f)
            _jumping = false;
    }


    public bool AttemptStateSwitch(CharacterStateSwitch3D state)
    {
        if (state.NewState is GroundState3D || state.NewState is DashState3D)
        {
            _controller.ChangeCharacterState(state);
            return true;
        }

        return false;
    }
}