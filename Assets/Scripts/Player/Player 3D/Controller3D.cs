using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerAttributes))]
public class Controller3D : MonoBehaviour
{
    public Transform CameraTransform;

    private Vector3 spawnPosition;
    private CharacterController characterController;
    private ICharacterState3D characterState;
    private int selectedAbility;
    private bool invincible;
    private float invincibleTime;
    private bool visible;

    public PlayerAttributes Attributes { get; private set; }
    public Vector3 Velocity { get; set; }
    public float MaxTraversableSlopeAngle { get { return characterController.slopeLimit; } }
    public float ColliderHeight { get { return characterController.height; } }
    public Vector2 MovementInput { get; private set; }

	public Animator animator; //!!!
    public Material Material;

	void Start(){
		
		animator = GetComponent<Animator> (); //!!!
	}

    private void Awake()
    {
        selectedAbility = 0;
        CacheComponents();
        SetInitialCharacterState();
        MovementInput = Vector2.zero;
        spawnPosition = transform.position;
        if (Attributes.Abilities.Count <= 0)
        {
            //var material = GetComponent<Renderer>().sharedMaterial;
            var abilityColor = Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors;
            //material.color = abilityColor.DefaultColor;
            Material.SetColor("_Color", abilityColor.DefaultColor);
        }
        invincible = false;
        invincibleTime = 0f;
    }

    private void OnApplicationQuit()
    {
        //var material = GetComponent<Renderer>().sharedMaterial;
        var abilityColor = Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors;
        //material.color = abilityColor.DefaultColor;
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
                //GetComponent<MeshRenderer>().enabled = visible;
                var renders = GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var r in renders)
                {
                    r.enabled = visible;
                }
            }
            if (invincibleTime >= Attributes.InvincibleTimeOnDamage)
            {
                invincible = false;
                invincibleTime = 0f;
                //GetComponent<MeshRenderer>().enabled = true;
                var renders = GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var r in renders)
                {
                    r.enabled = true;
                }
            }
        }
		if (animator != null){ //!!!
			UpdateAnimator(); //!!!
		}
    }

    public void RefreshMaterial()
    {
        var material = GetComponent<Renderer>().sharedMaterial;
        material.color = Attributes.Abilities[selectedAbility].Color;
        Material.SetColor("_Color", Attributes.Abilities[selectedAbility].Color);


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

	private void UpdateAnimator(){
		animator.SetFloat("VelocityX", transform.InverseTransformDirection(Velocity).x);
		animator.SetFloat ("VelocityY", transform.InverseTransformDirection(Velocity).z);
	}

    public void NextAbility()
    {
        if (selectedAbility + 1 < Attributes.Abilities.Count)
        {
            var material = GetComponent<Renderer>().sharedMaterial;
            material.color = Attributes.Abilities[++selectedAbility].Color;
            Material.color = Attributes.Abilities[++selectedAbility].Color;
            Material.SetColor("_Color", Attributes.Abilities[++selectedAbility].Color);
        }
    }

    public void PreviousAbility()
    {
        if (selectedAbility - 1 >= 0 && Attributes.Abilities.Count > 0)
        {
            var material = GetComponent<Renderer>().sharedMaterial;
            material.color = Attributes.Abilities[--selectedAbility].Color;
            Material.SetColor("_Color", Attributes.Abilities[--selectedAbility].Color);


        }
    }

    public void SetAbility(int index)
    {
        if (index < Attributes.Abilities.Count)
        {
            selectedAbility = index;
            //var material = GetComponent<Renderer>().sharedMaterial;
            //material.color = Attributes.Abilities[selectedAbility].Color;
            Material.SetColor("_Color", Attributes.Abilities[selectedAbility].Color);
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
        transform.position = position;
    }

    public void SetSpawn()
    {
        spawnPosition = transform.position;
    }

    public void AttackPlayer(Vector3 hitboxCenter, int damage)
    {
        if (invincible) return;
        Debug.Log("Player was attacked");
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
        Debug.Log("Current: " + transform.position + "\tSpawn: " + spawnPosition);
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