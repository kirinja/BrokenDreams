﻿using UnityEngine;

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
        if (useAbility) return;
        if (Input.GetJoystickNames().Length == 0)
        {
            useAbility = Input.GetButtonDown("Use Ability");
        }
        else
        {
            useAbility = Input.GetAxisRaw("Use Ability") > 0f;
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