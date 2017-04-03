using UnityEngine;

[RequireComponent(typeof(Controller3D))]
public class Input3D : MonoBehaviour
{
    private Controller3D player;
    private bool useAbility;
    private float previousAbilityAxis;

    private void Start()
    {
        player = GetComponent<Controller3D>();
        player.Is3D = true;
    }

    private void Update()
    {
        if (useAbility) return;

        if (previousAbilityAxis <= float.Epsilon)
        {
            useAbility = Input.GetAxisRaw("Use Ability") > float.Epsilon;
        }
        previousAbilityAxis = Input.GetAxisRaw("Use Ability");
    }

    private void FixedUpdate()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.HandleMovement(useAbility, input, false);
        useAbility = false;
    }
}