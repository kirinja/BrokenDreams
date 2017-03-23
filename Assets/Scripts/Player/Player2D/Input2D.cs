using UnityEngine;

[RequireComponent(typeof(Controller3D))]
public class Input2D : MonoBehaviour
{
    public float LockedZPosition = -1;

    private Controller3D controller;
    private bool useAbility;

    private void Start()
    {
        controller = GetComponent<Controller3D>();
    }

    private void Update()
    {
        if (!useAbility)
        {
            useAbility = Input.GetButtonDown("Use Ability");
        }
        if (Input.GetButtonDown("Next Ability"))
        {
            controller.NextAbility();
        }
        if (Input.GetButtonDown("Previous Ability"))
        {
            controller.PreviousAbility();
        }
    }

    private void FixedUpdate()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
        controller.HandleMovement(useAbility, input);
        controller.SetPosition(new Vector3(controller.transform.position.x, controller.transform.position.y,
            LockedZPosition));
        useAbility = false;
    }
}