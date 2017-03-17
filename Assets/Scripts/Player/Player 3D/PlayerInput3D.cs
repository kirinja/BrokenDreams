using System;
using UnityEngine;

[RequireComponent(typeof(PlayerController3D))]
public class PlayerInput3D : MonoBehaviour
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
            Debug.Log(useAbility);
        }
    }

    private void FixedUpdate()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        player.HandleMovement(useAbility, jump, h, v);
        jump = false;
        useAbility = false;
    }
}