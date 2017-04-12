using UnityEngine;

[RequireComponent(typeof(Controller3D))]
public class Input2D : MonoBehaviour
{
    public float LockedZPosition = -1;

    private Controller3D controller;
    private bool useAbility;
    private float previousAbilityAxis;

    private void Start()
    {
        controller = GetComponent<Controller3D>();
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
        controller.HandleMovement(useAbility, input);
        controller.SetPosition(new Vector3(controller.transform.position.x, controller.transform.position.y,
            LockedZPosition));
        useAbility = false;
    }
}