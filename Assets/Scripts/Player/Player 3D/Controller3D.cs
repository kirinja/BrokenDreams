using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Controller3D : MonoBehaviour
{
    [Tooltip("Degrees per second")]
    public float RotationSpeed = 120.0f;
    [Tooltip("Units per second when jumping while sliding down a slipe")]
    public float SlideJumpSpeed = 10.0f;
    [Tooltip("Maximum downwards velocity (entered as a positive value)")]
    public float TerminalVelocity = 18.0f;

    // TODO: Byt ut deaccelerationscale mot deaccelerationtime
    public float GroundDeaccelerationScale = 0.8f;public float AirDeaccelerationScale = 0.2f;

    private CharacterController characterController;
    private Velocity3D velocity;
    private ICharacterState3D characterState;
    private int selectedAbility;

    public PlayerAttributes Attributes { get; private set; }
    public float MaxJumpVelocity { get; private set; }
    public float MinJumpVelocity { get; private set; }
    public float Gravity { get; private set; }
    public Velocity3D Velocity { get { return characterState.Velocity; } }

    public float MaxTraversableSlopeAngle
    {
        get { return characterController.slopeLimit; }  
    }

    public float ColliderHeight
    {
        get { return characterController.height; }
    }

    private void Awake()
    {
        Attributes = GetComponent<PlayerAttributes>();
        selectedAbility = 0;
        CreateVelocity();
        CacheComponents();
        CalculateGravity();
        CalculateJumpVelocities();
        SetInitialCharacterState();
    }

    public void HandleMovement(bool useAbility, float h, float v)
    {
        foreach (var ability in Attributes.Abilities)
        {
            ability.UpdateTime();
        }
        if (useAbility && Attributes.Abilities.Count > selectedAbility && Attributes.Abilities[selectedAbility].CanUse)
        {
            var state = Attributes.Abilities[selectedAbility].Use(this);
            if (state.NewState != null)
            {
                characterState.AttemptStateSwitch(state);
            }
        }
        
        var deltaTime = Time.deltaTime;
        characterState.Update(h, v, deltaTime);
        HandleCollisions(Move());
        DrawAxes();
    }

    public void NextAbility()
    {
        if (selectedAbility + 1 < Attributes.Abilities.Count)
        {
            var material = GetComponent<Renderer>().sharedMaterial;
            material.color = Attributes.Abilities[++selectedAbility].Color;
        }
    }

    public void PreviousAbility()
    {
        if (selectedAbility - 1 >= 0 && Attributes.Abilities.Count > 0)
        {
            var material = GetComponent<Renderer>().sharedMaterial;
            material.color = Attributes.Abilities[--selectedAbility].Color;
        }
    }

    public void SetAbility(int index)
    {
        if (index < Attributes.Abilities.Count)
        {
            selectedAbility = index;
            var material = GetComponent<Renderer>().sharedMaterial;
            material.color = Attributes.Abilities[selectedAbility].Color;
        }
    }

    public void ChangeCharacterState(CharacterStateSwitch3D stateSwitch)
    {
        PrintStateSwitch(stateSwitch);

        characterState.Exit();
        characterState = stateSwitch.NewState;
        characterState.Enter();
        if (stateSwitch.RunImmediately)
        {
            characterState.Update(stateSwitch.MovementInput.x, stateSwitch.MovementInput.z, stateSwitch.DeltaTime);
        }
    }

    public bool IsTraversableSlope(float maxDistance)
    {
        var groundSlopeAngle = 0.0f;
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, maxDistance))
        {
            groundSlopeAngle = Vector3.Angle(Vector3.up, hitInfo.normal);
        }

        return groundSlopeAngle <= MaxTraversableSlopeAngle;
    }

    private void CacheComponents()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void CalculateGravity()
    {
        Gravity = (-2 * Attributes.MaxJumpHeight * Mathf.Pow(Attributes.MaxSpeed, 2)) /
                  (Mathf.Pow(Attributes.MaxJumpLength / 2, 2));
    }

    private void CalculateJumpVelocities()
    {
        var positiveGravity = Mathf.Abs(Gravity);
        MaxJumpVelocity = ((2 * Attributes.MaxJumpHeight * Attributes.MaxSpeed) / (Attributes.MaxJumpLength / 2));
        MinJumpVelocity = Mathf.Sqrt(2 * positiveGravity * Attributes.MinJumpHeight);
    }

    private void CreateVelocity()
    {
        velocity = new Velocity3D(-TerminalVelocity);
    }

    private void SetInitialCharacterState()
    {
        if (characterController.isGrounded)
        {
            if (IsTraversableSlope(ColliderHeight * 10.0f))
            {
                characterState = new AirState3D(this, velocity);
            }
            else
            {
                characterState = new GroundState3D(this, velocity);
            }
        }
        else
        {
            characterState = new AirState3D(this, velocity);
        }
    }

    private CollisionFlags Move()
    {
        var moveDirection = transform.TransformDirection(velocity.Current).normalized;
        var moveLength = velocity.Current.magnitude;
        var motion = moveDirection * moveLength;
        return characterController.Move(motion);
    }

    private void HandleCollisions(CollisionFlags collisionFlags)
    {
        var stateSwitch = characterState.HandleCollisions(collisionFlags);
        if (stateSwitch.NewState != null)
        {
            ChangeCharacterState(stateSwitch);
        }
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void DrawAxes()
    {
        Debug.DrawRay(transform.position + transform.forward * characterController.radius, transform.forward, Color.blue);
        Debug.DrawRay(transform.position + transform.right * characterController.radius, transform.right, Color.red);
        Debug.DrawRay(transform.position + transform.up * characterController.height * 0.5f, transform.up, Color.green);
    }

    private void PrintStateSwitch(CharacterStateSwitch3D stateSwitch)
    {
        print("Switching character state from " + characterState.ToString() + " to " + stateSwitch.NewState.ToString());
    }
}