using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerAttributes))]
public class Controller3D : MonoBehaviour
{
    public Transform CameraTransform;

    private CharacterController characterController;
    private ICharacterState3D characterState;
    private int selectedAbility;

    public PlayerAttributes Attributes { get; private set; }
    public Vector3 Velocity { get; set; }
    public float MaxTraversableSlopeAngle { get { return characterController.slopeLimit; } }
    public float ColliderHeight { get { return characterController.height; } }
    public Vector2 MovementInput { get; private set; }

    private void Awake()
    {
        selectedAbility = 0;
        CacheComponents();
        SetInitialCharacterState();
        MovementInput = Vector2.zero;
    }

    public void HandleMovement(bool useAbility, Vector2 input)
    {
        MovementInput = input;
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
        characterState.Update(input);
        HandleCollisions(Move());
        DrawAxes();
		GetComponentInChildren<Rigidbody> ().position = transform.position;
		GetComponentInChildren<Rigidbody> ().rotation = transform.rotation;
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

    public void SetPosition(Vector3 position)
    {
        this.transform.position = position;
    }

    private void CacheComponents()
    {
        characterController = GetComponent<CharacterController>();
        Attributes = GetComponent<PlayerAttributes>();
    }

    private void SetInitialCharacterState()
    {
        if (characterController.isGrounded)
        {
            if (IsTraversableSlope(ColliderHeight * 10.0f))
            {
                characterState = new AirState3D(this);
            }
            else
            {
                characterState = new GroundState3D(this);
            }
        }
        else
        {
            characterState = new AirState3D(this);
        }
    }

    private CollisionFlags Move()
    {
        return characterController.Move(Velocity * Time.deltaTime);
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