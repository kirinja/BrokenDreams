using System.Diagnostics;
using Prime31.TransitionKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerAttributes))]
[RequireComponent(typeof(Animator))]
public class Controller3D : MonoBehaviour
{
    private bool _abilityColorActive;
    private Timer _abilityColorTimer;
    private CharacterController _characterController;
    private ICharacterState3D _characterState;
    private bool _invincible;
    private float _invincibleTime;
    private int _latestUsedAbility;
    private Vector3 _spawnPosition;
    private bool _visible;

    public AbilityGUI AbilityUI;
    public LayerMask GroundMask;
    public float LockedZPosition = -1;
    public Material Material;

    public PlayerAttributes Attributes { get; private set; }
    public Vector2 Velocity { get; set; }
    public Vector2 Forward { get; set; }
    public Vector2 MovementInput { get; private set; }
    public Animator Animator { get; private set; }
    public bool CollideSide { get; set; } // Ugly
    public bool CollideDown { get; set; }
    public bool CollideUp { get; set; }


    public Vector3 SpawnPoint
    {
        get { return _spawnPosition; }
    }


    public float ColliderHeight
    {
        get { return _characterController.height; }
    }


    public float ColliderWidth
    {
        get { return _characterController.radius * 2; }
    }


    private float MaxTraversableSlopeAngle
    {
        get { return _characterController.slopeLimit; }
    }


    public Vector2 Gravity
    {
        get
        {
            return Vector2.up *
                   (-2 * GetComponent<PlayerAttributes>().MaxJumpHeight *
                    Mathf.Pow(GetComponent<PlayerAttributes>().MaxSpeed, 2)) /
                   Mathf.Pow(GetComponent<PlayerAttributes>().MaxJumpLength / 2, 2);
        }
    }


    public float MaxJumpVelocity
    {
        get
        {
            return 2 * GetComponent<PlayerAttributes>().MaxJumpHeight * GetComponent<PlayerAttributes>().MaxSpeed /
                   (GetComponent<PlayerAttributes>().MaxJumpLength / 2);
        }
    }


    public float MinJumpVelocity
    {
        get { return Mathf.Sqrt(2 * Gravity.magnitude * GetComponent<PlayerAttributes>().MinJumpHeight); }
    }


    private void Awake()
    {
        _abilityColorTimer = new Timer(2f);
        _latestUsedAbility = 0;
        CacheComponents();
        SetInitialCharacterState();
        MovementInput = Vector2.zero;
        _spawnPosition = transform.position;

        ResetColor();

        _invincible = false;
        _invincibleTime = 0f;
    }


    private void Start()
    {
        transform.position = _spawnPosition;
        Animator = GetComponent<Animator>();
        Forward = transform.forward;
        _abilityColorActive = false;
    }


    public void Spawn()
    {
        transform.position = _spawnPosition;
    }


    private void Update()
    {
        if (GameManager.IsPaused()) return;

        MovementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        UseAbilities();
        _characterState.Update();
        
        UpdateInvincibility();
        UpdateAbilityColor();
        UpdateAnimator();
        DrawAxes();
    }


    private void LateUpdate()
    {
        if (GameManager.IsPaused()) return;

        _characterState.LateUpdate();
    }


    private void FixedUpdate()
    {
        if (GameManager.IsPaused()) return;

        _characterState.FixedUpdate();
        HandleCollisions(Move());
        GetComponentInChildren<Rigidbody>().position = transform.position;
        transform.position =
            new Vector3(transform.position.x, transform.position.y, LockedZPosition); // Lock the player's Z position
    }


    private void OnApplicationQuit()
    {
        ResetColor();
    }


    private void UseAbilities()
    {
        var abilityInputs = new bool[4];
        for (var i = 0; i < abilityInputs.Length; ++i)
            abilityInputs[i] = Input.GetButtonDown("Use Ability " + (i + 1)) || abilityInputs[i];

        var usedAbilities = new bool[4];
        for (var i = 0; i < Attributes.Abilities.Count; ++i)
        {
            Attributes.Abilities[i].UpdateAbility();
            if (Attributes.Abilities[i].CanUse && abilityInputs[i])
            {
                var state = Attributes.Abilities[i].Use(this);
                if (state.NewState != null)
                {
                    if (_characterState.AttemptStateSwitch(state))
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
    }


    private void UpdateInvincibility()
    {
        if (_invincible)
        {
            _invincibleTime += Time.deltaTime;
            if (_invincibleTime % 0.2f < 0.1f)
            {
                _visible = !_visible;
                var renders = GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var r in renders)
                    r.enabled = _visible;
            }

            if (_invincibleTime >= Attributes.InvincibleTimeOnDamage)
            {
                _invincible = false;
                _invincibleTime = 0f;
                var renders = GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var r in renders)
                    r.enabled = true;
            }
        }
    }


    private void UpdateAbilityColor()
    {
        if (_abilityColorActive)
            if (_abilityColorTimer.Update(Time.deltaTime))
            {
                var abilityColor = Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors;
                if (abilityColor != null) Material.SetColor("_Color", abilityColor.DefaultColor);
                _abilityColorActive = false;
            }
            else
            {
                var abilityColor = Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors;
                if (abilityColor != null)
                    Material.SetColor("_Color",
                        Color.Lerp(Attributes.Abilities[_latestUsedAbility].Color, abilityColor.DefaultColor,
                            _abilityColorTimer.PercentDone));
            }
    }


    private void ResetColor()
    {
        var abilityColor = Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors;
        if (abilityColor != null) Material.SetColor("_Color", abilityColor.DefaultColor);
    }


    private void SetAbilityActivated(int index)
    {
        _latestUsedAbility = index;
        Material.SetColor("_Color", Attributes.Abilities[index].Color);
        _abilityColorActive = true;
        _abilityColorTimer.Reset();
    }


    private void UpdateAnimator()
    {
        Animator.SetFloat("VelocityX", transform.InverseTransformDirection(Velocity).x);
        Animator.SetFloat("VelocityY", transform.InverseTransformDirection(Velocity).z);
    }


    public void ChangeCharacterState(CharacterStateSwitch3D stateSwitch)
    {
        PrintStateSwitch(stateSwitch);

        _characterState.Exit();
        _characterState = stateSwitch.NewState;
        _characterState.Enter();
    }


    public bool IsTraversableSlope(float maxDistance)
    {
        var groundSlopeAngle = 0.0f;
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, maxDistance))
            groundSlopeAngle = Vector3.Angle(Vector3.up, hitInfo.normal);

        return groundSlopeAngle <= MaxTraversableSlopeAngle;
    }


    public void SetSpawn(Vector3 position)
    {
        _spawnPosition = position;
    }


    public void AttackPlayer(Vector3 hitboxCenter, int damage)
    {
        if (_invincible) return;

        _invincible = true;
        KnockAway(hitboxCenter);
        Damage(damage);
    }


    private void KnockAway(Vector3 hitboxCenter)
    {
        var direction = transform.position - hitboxCenter;
        Velocity = direction.normalized * Attributes.KnockbackVelocity + Vector3.up * 10;
        ChangeCharacterState(new CharacterStateSwitch3D(new AirState3D(this)));
    }


    public void Damage(int damage = 1)
    {
        GetComponent<PlayerHealth>().TakeDamage(damage);
    }


    public void Kill()
    {
        /*ChangeCharacterState(new CharacterStateSwitch3D(new GroundState3D(this)));
        transform.position = _spawnPosition;
        Velocity = Vector3.zero;

        // TODO: Respawn from somewhere else?
        GetComponent<PlayerHealth>().Respawn();
        /**/
        
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // problem since we can still move during the transition
        var wind = new WindTransition()
        {
            nextScene = SceneManager.GetActiveScene().name,
            duration = 0.5f,
            size = 0.3f
        };
        TransitionKit.instance.transitionWithDelegate(wind);
    }


    private void CacheComponents()
    {
        _characterController = GetComponent<CharacterController>();
        Attributes = GetComponent<PlayerAttributes>();
    }


    private void SetInitialCharacterState()
    {
        if (_characterController.isGrounded)
            if (IsTraversableSlope(ColliderHeight * 10.0f))
                _characterState = new AirState3D(this);
            else
                _characterState = new GroundState3D(this);
        else
            _characterState = new AirState3D(this);
    }


    private CollisionFlags Move()
    {
        var timedMove = Velocity * Time.deltaTime;
        return _characterController.Move(new Vector3(timedMove.x, timedMove.y, 0f));
    }


    private void HandleCollisions(CollisionFlags collisionFlags)
    {
        var stateSwitch = _characterState.HandleCollisions(collisionFlags);
        if (stateSwitch.NewState != null)
            ChangeCharacterState(stateSwitch);
    }


    [Conditional("UNITY_EDITOR")]
    private void DrawAxes()
    {
        Debug.DrawRay(transform.position + transform.forward * _characterController.radius, transform.forward,
            Color.blue);
        Debug.DrawRay(transform.position + transform.right * _characterController.radius, transform.right, Color.red);
        Debug.DrawRay(transform.position + transform.up * _characterController.height * 0.5f, transform.up,
            Color.green);
    }


    private void PrintStateSwitch(CharacterStateSwitch3D stateSwitch)
    {
        print("Switching character state from " + _characterState + " to " + stateSwitch.NewState);
    }
}