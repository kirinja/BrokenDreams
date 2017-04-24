using System;
using UnityEngine;

// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

public struct GroundState3D : ICharacterState3D
{
    private readonly Controller3D _controller;
    private Transform _platform;
    private Vector3 _previousPlatformPosition;
    //private Vector3 _scale;


    public GroundState3D(Controller3D controller)
    {
        if (controller == null)
            throw new ArgumentNullException("controller");

        
        _controller = controller;
        _platform = null;
        _previousPlatformPosition = Vector3.zero;
    }


    public void Enter()
    {
        _controller.Velocity = new Vector2(_controller.Velocity.x, 0f);
    }


    public void Update()
    {
        var desiredVelocity = new Vector2(
            _controller.MovementInput.x * _controller.GetComponent<PlayerAttributes>().MaxSpeed, 0f);

        UpdateRotation();
        ApplyAcceleration(desiredVelocity);
        ApplyFriction(desiredVelocity);
        ApplyGravity();

        if (Input.GetButtonDown("Taunt"))
            _controller.Animator.SetTrigger("Wave");

        
    }


    public void Exit()
    {
        _controller.transform.SetParent(null);
    }


    public void LateUpdate()
    {
        
    }


    public void FixedUpdate()
    {
        ClampAsChild();
    }


    private void ClampToPlatform()
    {
        var gameObject = GetGround();
        if (gameObject)
        {
            if (gameObject.transform == _platform)
            {
                _controller.transform.position += _platform.position - _previousPlatformPosition;
                _previousPlatformPosition = _platform.position;
            }
            else
            {
                _platform = gameObject.transform;
                _previousPlatformPosition = _platform.position;
            }
        }
        else
        {
            _platform = null;
            // TODO: delet dis
            Debug.Log("NO PLARTFROM");
        }
    }


    private void ClampAsChild()
    {
        var gameObject = GetGround();
        Debug.Log(gameObject.tag);
        if (gameObject && gameObject.CompareTag("Moving"))
        {
            _controller.transform.SetParent(gameObject.transform.parent);
        }
        else
        {
            _controller.transform.SetParent(null);
        }
    }


    public CharacterStateSwitch3D HandleCollisions(CollisionFlags collisionFlags)
    {
        CharacterStateSwitch3D stateSwitch;
        if ((collisionFlags & CollisionFlags.Below) == CollisionFlags.Below || IsGrounded())
        {
            _controller.Velocity = new Vector2(_controller.Velocity.x, 0f);
            stateSwitch = new CharacterStateSwitch3D();
        }
        else
        {
            stateSwitch = new CharacterStateSwitch3D(new AirState3D(_controller));
        }
        if ((collisionFlags & CollisionFlags.Sides) == CollisionFlags.Sides)
            _controller.Velocity = new Vector2(0f, _controller.Velocity.y);
        if ((collisionFlags & CollisionFlags.Above) == CollisionFlags.Above)
            _controller.Velocity = new Vector2(_controller.Velocity.x, 0f);

        return stateSwitch;
    }


    private bool IsGrounded()
    {
        return Physics.Raycast(
                   new Vector3(_controller.transform.position.x - _controller.ColliderWidth / 2,
                       _controller.transform.position.y, _controller.transform.position.z), Vector3.down,
                   _controller.ColliderHeight / 2, _controller.GroundMask) ||
               Physics.Raycast(
                   new Vector3(_controller.transform.position.x + _controller.ColliderWidth / 2,
                       _controller.transform.position.y, _controller.transform.position.z), Vector3.down,
                   _controller.ColliderHeight / 2, _controller.GroundMask);
    }


    private void UpdateRotation()
    {
        var currentAngle = _controller.transform.eulerAngles.y;
        var desiredAngle = Mathf.Sign(_controller.Forward.x) > 0f ? 90f : -90f;
        if (Mathf.Abs(_controller.MovementInput.x) > 0)
        {
            desiredAngle = Mathf.Sign(_controller.MovementInput.x) > 0f ? 90f : -90f;
            _controller.Forward = new Vector2(_controller.MovementInput.x, 0f);
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
                           (attributes.MaxSpeed / attributes.GroundAccelerationTime);

        if (desiredVelocity.x > 0f && _controller.Velocity.x + acceleration > desiredVelocity.x ||
            desiredVelocity.x < 0f && _controller.Velocity.x + acceleration < desiredVelocity.x)
            acceleration = desiredVelocity.x - _controller.Velocity.x;
        _controller.Velocity += Vector2.right * acceleration;
    }


    private void ApplyFriction(Vector2 desiredVelocity)
    {
        var attributes = _controller.GetComponent<PlayerAttributes>();
        var deacceleration = attributes.MaxSpeed / attributes.GroundDeaccelerationTime;
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


    private GameObject GetGround()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(_controller.transform.position /*+ Vector3.down * (_controller.ColliderHeight / 2 - 0.1f)/**/,
            Vector3.down, out hitInfo, 50, _controller.GroundMask))
        {
            return hitInfo.transform.gameObject;
        }

        return null;
    }


    public bool AttemptStateSwitch(CharacterStateSwitch3D state)
    {
        if (state.NewState == null)
            return false;

        if (state.NewState is AirState3D || state.NewState is DashState3D)
        {
            _controller.ChangeCharacterState(state);
            return true;
        }

        return false;
    }
}