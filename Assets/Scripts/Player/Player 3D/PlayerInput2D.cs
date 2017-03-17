using System;
using UnityEngine;

[RequireComponent(typeof(PlayerController3D))]
public class PlayerInput2D : MonoBehaviour
{
    private PlayerController3D player;
    private bool jump, useAbility;

    private void Start()
    {
        player = GetComponent<PlayerController3D>();
    }

    private void Update()
    {
        if (!jump)
        {
            jump = Input.GetButtonDown("Jump");
        }

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

        player.HandleMovement(useAbility, jump, h, 0f);
        jump = false;
        useAbility = false;
    }
}