using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerAttributes))]
[RequireComponent(typeof(Animator))]
public class Controller3D : MonoBehaviour
{
    public Transform CameraTransform;
    public Material Material;
    public AbilityGUI AbilityUI;
    public LayerMask GroundMask;

    private CharacterController characterController;
    private ICharacterState3D characterState;
    private bool invincible;
    private float invincibleTime;
    private int latestUsedAbility;
    private Vector3 spawnPosition;
    private bool visible;
    private bool abilityColorActive;
    private Timer abilityColorTimer;

    public PlayerAttributes Attributes { get; private set; }
    public Vector2 Velocity { get; set; }
    public Vector2 Forward { get; set; }
    public Vector2 MovementInput { get; private set; }
    public Animator Animator { get; private set; }

    public float ColliderHeight { get { return characterController.height; } }
    public float ColliderWidth { get { return characterController.radius * 2; } }
    public float MaxTraversableSlopeAngle { get { return characterController.slopeLimit; } }
    public Vector2 Gravity { get { return Vector2.up * (-2 * GetComponent<PlayerAttributes>().MaxJumpHeight * Mathf.Pow(GetComponent<PlayerAttributes>().MaxSpeed, 2)) / Mathf.Pow(GetComponent<PlayerAttributes>().MaxJumpLength / 2, 2); } }
    public float MaxJumpVelocity { get { return 2 * GetComponent<PlayerAttributes>().MaxJumpHeight * GetComponent<PlayerAttributes>().MaxSpeed / (GetComponent<PlayerAttributes>().MaxJumpLength / 2); } }
    public float MinJumpVelocity { get { return Mathf.Sqrt(2 * Gravity.magnitude * GetComponent<PlayerAttributes>().MinJumpHeight); } }

    private void Start()
    {
        Animator = GetComponent<Animator>();
        Forward = transform.forward;
        abilityColorActive = false;
    }

    private void Awake()
    {
        abilityColorTimer = new Timer(2f);
        latestUsedAbility = 0;
        CacheComponents();
        SetInitialCharacterState();
        MovementInput = Vector2.zero;
        spawnPosition = transform.position;

        ResetColor();

        invincible = false;
        invincibleTime = 0f;
    }

    private void OnApplicationQuit()
    {
        ResetColor();
    }

    private void ResetColor()
    {
        var abilityColor = Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors;
        Material.SetColor("_Color", abilityColor.DefaultColor);
    }

    private void Update()
    {
        if (invincible)
        {
            invincibleTime += Time.deltaTime;
            if (invincibleTime % 0.2f < 0.1f)
            {
                visible = !visible;
                var renders = GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var r in renders)
                    r.enabled = visible;
            }
            if (invincibleTime >= Attributes.InvincibleTimeOnDamage)
            {
                invincible = false;
                invincibleTime = 0f;
                var renders = GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var r in renders)
                    r.enabled = true;
            }
        }

        if (abilityColorActive)
        {
            if (abilityColorTimer.Update(Time.deltaTime))
            {
                var abilityColor = Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors;
                Material.SetColor("_Color", abilityColor.DefaultColor);
                abilityColorActive = false;
            }
            else
            {
                var abilityColor = Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors;
                Material.SetColor("_Color", Color.Lerp(Attributes.Abilities[latestUsedAbility].Color, abilityColor.DefaultColor, abilityColorTimer.PercentDone));
            }
        }

        UpdateAnimator();
    }

    public void HandleMovement(bool[] useAbility, Vector2 input)
    {
        MovementInput = input;
        var usedAbilities = new bool[4];
        for (var i = 0; i < Attributes.Abilities.Count; ++i)
        {
            Attributes.Abilities[i].UpdateTime();
            if (Attributes.Abilities[i].CanUse && useAbility[i])
            {
                var state = Attributes.Abilities[i].Use(this);
                if (state.NewState != null)
                {
                    if (characterState.AttemptStateSwitch(state))
                    {
                        usedAbilities[i] = true;
                        SetAbilityActivated(i);
                    }
                }
                else
                {
                    usedAbilities[i] = true;
                    SetAbilityActivated(i);
                }
            }
        }

        AbilityUI.ShowAbilitiesUsed(usedAbilities);

        characterState.Update(input);
        HandleCollisions(Move());
        DrawAxes();
        GetComponentInChildren<Rigidbody>().position = transform.position;
    }

    private void SetAbilityActivated(int index)
    {
        latestUsedAbility = index;
        Material.SetColor("_Color", Attributes.Abilities[index].Color);
        abilityColorActive = true;
        abilityColorTimer.Reset();
    }

    private void UpdateAnimator()
    {
        Animator.SetFloat("VelocityX", transform.InverseTransformDirection(Velocity).x);
        Animator.SetFloat("VelocityY", transform.InverseTransformDirection(Velocity).z);
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
            groundSlopeAngle = Vector3.Angle(Vector3.up, hitInfo.normal);

        return groundSlopeAngle <= MaxTraversableSlopeAngle;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetSpawn()
    {
        spawnPosition = transform.position;
    }

    public void SetSpawn(Vector3 position)
    {
        spawnPosition = position;
    }

    public void AttackPlayer(Vector3 hitboxCenter, int damage)
    {
        if (invincible) return;
        invincible = true;
        KnockAway(hitboxCenter);
        Damage(damage);
    }

    public void KnockAway(Vector3 hitboxCenter)
    {
        var direction = transform.position - hitboxCenter;
        Velocity = direction.normalized * Attributes.KnockbackVelocity + Vector3.up * 10;
        ChangeCharacterState(new CharacterStateSwitch3D(new AirState3D(this)));
    }

    public void Damage()
    {
        GetComponent<PlayerHealth>().TakeDamage(1);
    }

    public void Damage(int damage)
    {
        GetComponent<PlayerHealth>().TakeDamage(damage);
    }

    public void Kill()
    {
        ChangeCharacterState(new CharacterStateSwitch3D(new GroundState3D(this)));
        transform.position = spawnPosition;
        Velocity = Vector3.zero;

        // TODO: Respawn from somewhere else?
        GetComponent<PlayerHealth>().Respawn();
    }

    private void CacheComponents()
    {
        characterController = GetComponent<CharacterController>();
        Attributes = GetComponent<PlayerAttributes>();
    }

    private void SetInitialCharacterState()
    {
        if (characterController.isGrounded)
            if (IsTraversableSlope(ColliderHeight * 10.0f))
                characterState = new AirState3D(this);
            else
                characterState = new GroundState3D(this);
        else
            characterState = new AirState3D(this);
    }

    private CollisionFlags Move()
    {
        var timedMove = Velocity * Time.deltaTime;
        return characterController.Move(new Vector3(timedMove.x, timedMove.y, 0f));
    }

    private void HandleCollisions(CollisionFlags collisionFlags)
    {
        var stateSwitch = characterState.HandleCollisions(collisionFlags);
        if (stateSwitch.NewState != null)
            ChangeCharacterState(stateSwitch);
    }

    [Conditional("UNITY_EDITOR")]
    private void DrawAxes()
    {
        Debug.DrawRay(transform.position + transform.forward * characterController.radius, transform.forward,
            Color.blue);
        Debug.DrawRay(transform.position + transform.right * characterController.radius, transform.right, Color.red);
        Debug.DrawRay(transform.position + transform.up * characterController.height * 0.5f, transform.up, Color.green);
    }

    private void PrintStateSwitch(CharacterStateSwitch3D stateSwitch)
    {
        print("Switching character state from " + characterState + " to " + stateSwitch.NewState);
    }
}