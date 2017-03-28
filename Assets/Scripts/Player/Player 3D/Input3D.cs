﻿using UnityEngine;

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
            if (!useAbility)
            {
                useAbility = Input.GetAxisRaw("Use Ability") > 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.HandleMovement(useAbility, input);
        useAbility = false;
    }
}