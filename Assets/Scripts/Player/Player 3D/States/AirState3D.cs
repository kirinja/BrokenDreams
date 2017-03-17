using System;
using UnityEngine;

public class AirState3D : IPlayerState3D
{
    bool shouldJump;

    public AirState3D(bool jump = false)
    {
        shouldJump = jump;
        Debug.Log("Switching state to: Air");
    }

    public void HandleMovement(PlayerController3D player, Vector2 input)
    {
        var move = new Vector3(input.x, 0, input.y);
        move.Normalize();
        var currentAngle = player.transform.eulerAngles.y;
        var inputAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
        var direction = player.CameraTransform.eulerAngles.y + inputAngle;
        var forwardMovement = move.magnitude;
        var attributes = player.GetComponent<PlayerAttributes>();
        var desiredForwardVelocity = forwardMovement * attributes.MaxSpeed;
        var acceleration = attributes.MaxSpeed / attributes.AirAccelerationTime;

        // Change character direction
        if (input.magnitude > float.Epsilon)
        {
            var rotationAngle = Mathf.Abs(direction - currentAngle);
            var rotationTime = attributes.MaxRotationTime / 180f * rotationAngle;
            var rotationSpeed = rotationAngle / rotationTime;
            //direction = Mathf.LerpAngle(currentAngle, direction, Mathf.Min(1f, Time.deltaTime * rotationSpeed));
            player.transform.eulerAngles = new Vector3(0, direction, 0);
        }
        var currentLocalVelocity = player.transform.InverseTransformDirection(player.Velocity);

        // Calculate and apply acceleration
        if (forwardMovement > float.Epsilon)
        {
            if (currentLocalVelocity.z < desiredForwardVelocity)
            {
                var accelerationAmount = Time.deltaTime * acceleration;
                if (currentLocalVelocity.z + accelerationAmount > desiredForwardVelocity)
                {
                    accelerationAmount = desiredForwardVelocity - currentLocalVelocity.z;
                }
                currentLocalVelocity.z += accelerationAmount; // Apply acceleration
            }
        }

        /*if (Mathf.Abs(move.z) > 0f)
        {
            // TODO: Desired velocity?
            currentVelocity.z += Time.deltaTime * Mathf.Sign(move.z) * acceleration;
            if (Mathf.Abs(currentVelocity.z) > attributes.MaxSpeed) currentVelocity.z = attributes.MaxSpeed * Mathf.Sign(currentVelocity.z);
            player.Velocity = currentVelocity;
        }
        if (Mathf.Abs(turnAmount) > 0f)
        {
            var localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
            localVelocity.x += Time.deltaTime * Mathf.Sign(-turnAmount) * AirAcceleration;
            if (Mathf.Abs(localVelocity.x) > MaxAirSpeed) localVelocity.x = MaxAirSpeed * Mathf.Sign(localVelocity.x);
            rigidbody.velocity = transform.TransformDirection(localVelocity);
        }*/

        if (shouldJump)
        {
            currentLocalVelocity.y = ((2 * attributes.MaxJumpHeight * attributes.MaxSpeed) / (attributes.MaxJumpLength / 2));
            shouldJump = false;
        }

        var gravity = new Vector3(0f, (-2 * attributes.MaxJumpHeight * Mathf.Pow(attributes.MaxSpeed, 2)) / (Mathf.Pow(attributes.MaxJumpLength / 2, 2)), 0f);
        currentLocalVelocity += gravity * Time.deltaTime;


        
        var collider = player.GetComponent<BoxCollider>();
        RaycastHit hitInfo;

        // X collision
        var velocityX = player.transform.TransformDirection(new Vector3(currentLocalVelocity.x, 0f, 0f));
        if (Physics.BoxCast(collider.transform.position, new Vector3(float.Epsilon, collider.size.y, collider.size.z / 2),
            velocityX.normalized, out hitInfo, collider.transform.rotation, collider.size.x / 2 + velocityX.magnitude * Time.deltaTime, player.CollisionMask))
        {
            player.transform.position += (hitInfo.distance - collider.size.x / 2) * velocityX.normalized;
            currentLocalVelocity = new Vector3(0f, currentLocalVelocity.y, currentLocalVelocity.z);
            Debug.Log("X collision in Ground state.");
        }
        else
        {
            player.transform.position += velocityX * Time.deltaTime;
        }

        // Z collision
        var velocityZ = player.transform.TransformDirection(new Vector3(0f, 0f, currentLocalVelocity.z));
        if (Physics.BoxCast(collider.transform.position, new Vector3(collider.size.x / 2, collider.size.y, float.Epsilon),
            velocityZ.normalized, out hitInfo, collider.transform.rotation, collider.size.z / 2 + velocityZ.magnitude * Time.deltaTime, player.CollisionMask))
        {
            player.transform.position += (hitInfo.distance - collider.size.z / 2) * velocityZ.normalized;
            currentLocalVelocity = new Vector3(currentLocalVelocity.x, currentLocalVelocity.y, 0f);
            Debug.Log("Z collision in Ground state.");
        }
        else
        {
            player.transform.position += velocityZ * Time.deltaTime;
        }

        // Y Collision
        var velocityY = player.transform.TransformDirection(new Vector3(0f, currentLocalVelocity.y, 0f));
        if (Physics.BoxCast(collider.transform.position, new Vector3(collider.size.x / 2, float.Epsilon, collider.size.z / 2),
            velocityY.normalized, out hitInfo, collider.transform.rotation, collider.size.y / 2 + velocityY.magnitude * Time.deltaTime, player.CollisionMask))
        {
            if (velocityY.y < 0f)
            {
                player.SwitchState(new GroundState3D());
            }
            player.transform.position += (hitInfo.distance - collider.size.y / 2) * velocityY.normalized;
            currentLocalVelocity = new Vector3(currentLocalVelocity.x, 0f, currentLocalVelocity.z);
            Debug.Log("Y collision in Air state.");
        }
        else
        {
            player.transform.position += velocityY * Time.deltaTime;
        }

        player.Velocity = player.transform.TransformDirection(currentLocalVelocity);
    }

    /*public override void HandleCollision(PlayerController3D player, Collision collision)
    {
        var normal = Vector3.zero;
        var point = Vector3.zero;
        var separation = 0f;
        foreach (var contact in collision.contacts)
        {
            normal += contact.normal;
            point += contact.point;
            separation += contact.separation;
        }
        normal.Normalize();
        point /= collision.contacts.Length;
        separation /= collision.contacts.Length;

        if (point.y < player.transform.position.y)
        {
            player.SwitchState(new GroundState3D());
        }

        player.transform.position -= normal * separation;
    }*/

    public void AttemptStateSwitch(PlayerController3D player, IPlayerState3D newState)
    {
        if (!(newState is AirState3D))
        {
            player.SwitchState(newState);
        }
    }
}
