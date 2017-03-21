using UnityEngine;

[RequireComponent(typeof(Controller3D))]
public class Input3D : MonoBehaviour
{
    private Controller3D player;
    private bool useAbility;

    private void Start()
    {
        player = GetComponent<Controller3D>();
    }

    private void Update()
    {
        if (!useAbility)
        {
            useAbility = Input.GetButtonDown("Use Ability");
        }
        if (Input.GetButtonDown("Next Ability"))
        {
            player.NextAbility();
        }
        if (Input.GetButtonDown("Previous Ability"))
        {
            player.PreviousAbility();
        }
    }

    private void FixedUpdate()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        player.HandleMovement(useAbility, h, v);
        useAbility = false;
    }
}