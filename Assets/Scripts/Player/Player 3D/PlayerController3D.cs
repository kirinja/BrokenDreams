using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(PlayerAttributes))]
public class PlayerController3D : MonoBehaviour
{
    public float TurnSpeed = 360;
    public Transform CameraTransform;
    public LayerMask CollisionMask;

    [HideInInspector]
    public Vector3 Velocity { get; set; }

    private PlayerAttributes attributes;
    private Vector3 groundNormal;
    private IPlayerState3D currentState;
    private int selectedAbility;

    public void Start()
    {
        attributes = GetComponent<PlayerAttributes>();
        selectedAbility = 0;
        currentState = new AirState3D();
        if (attributes.Abilities.Count > 0)
        {
            GetComponent<Renderer>().sharedMaterial.color = attributes.Abilities[selectedAbility].Color;
        }
        else
        {
            GetComponent<Renderer>().sharedMaterial.color =
                (Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors).DefaultColor;
        }
    }

    private void Update()
    {
        foreach (var ability in attributes.Abilities)
        {
            ability.UpdateTime();
        }
    }

    public void NextAbility()
    {
        if (selectedAbility + 1 < attributes.Abilities.Count)
        {
            var material = GetComponent<Renderer>().sharedMaterial;
            material.color = attributes.Abilities[++selectedAbility].Color;
        }
    }

    public void PreviousAbility()
    {
        if (selectedAbility - 1 >= 0 && attributes.Abilities.Count > 0)
        {
            var material = GetComponent<Renderer>().sharedMaterial;
            material.color = attributes.Abilities[--selectedAbility].Color;
        }
    }

    public void HandleMovement(bool useAbility, bool jump, float horizontalInput, float verticalInput)
    {
        var input = new Vector2(horizontalInput, verticalInput);
        input.Normalize();

        if (useAbility && attributes.Abilities.Count > 0)
        {
            var state = attributes.Abilities[selectedAbility].Use(true);
            if (state != null)
            {
                currentState.AttemptStateSwitch(this, state as IPlayerState3D); // Safe to cast as Ability.Use(true) should always return a 3D state
            }
        }
        currentState.HandleMovement(this, input);
    }

    public void SwitchState(IPlayerState3D state)
    {
        currentState = state;
    }
}
