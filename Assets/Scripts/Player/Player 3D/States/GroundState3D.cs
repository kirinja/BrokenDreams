using System;
using UnityEngine;

public class GroundState3D : IPlayerState3D
{
    private const float GroundCheckDistance = 0.1f;

    public GroundState3D()
    {
        Debug.Log("Switching state to: Ground");
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
        var acceleration = attributes.MaxSpeed / attributes.GroundAccelerationTime;
        var deacceleration = attributes.MaxSpeed / attributes.GroundDeaccelerationTime;

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
                currentLocalVelocity.z += accelerationAmount;
            }
        }

        // Calculate and apply friction
        var invertedFrictionDirection = currentLocalVelocity - new Vector3(0, 0, desiredForwardVelocity);
        if ((int)Mathf.Sign(currentLocalVelocity.z) != (int)Mathf.Sign(invertedFrictionDirection.z))
        {
            invertedFrictionDirection.z = 0f;
        }
        var friction = -invertedFrictionDirection.normalized * deacceleration * Time.deltaTime;
        var newVelocity = currentLocalVelocity + friction;
        if (currentLocalVelocity.z > desiredForwardVelocity && desiredForwardVelocity > newVelocity.z)
        {
            newVelocity.z = desiredForwardVelocity;
        }
        if ((int)Mathf.Sign(newVelocity.x) != (int)Mathf.Sign(currentLocalVelocity.x)) newVelocity.x = 0f;
        if ((int)Mathf.Sign(newVelocity.y) != (int)Mathf.Sign(currentLocalVelocity.y)) newVelocity.y = 0f;
        if ((int)Mathf.Sign(newVelocity.z) != (int)Mathf.Sign(currentLocalVelocity.z)) newVelocity.z = 0f;
        currentLocalVelocity = newVelocity;


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
            player.transform.position += (hitInfo.distance - collider.size.y / 2) * velocityY.normalized;
            currentLocalVelocity = new Vector3(currentLocalVelocity.x, 0f, currentLocalVelocity.z);
            Debug.Log("Y collision in Ground state.");
        }
        else
        {
            if (velocityY.y < 0f)
            {
                player.SwitchState(new AirState3D());
            }
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

        player.transform.position -= normal * separation;
    }*/

    private bool CheckGroundStatus(BoxCollider collider, LayerMask collisionMask)
    {
        var rayCastInset = 0.1f;
        /*var raycastPoints = new Vector3[1];
        // TODO: Maybe consider collider's center as well
        raycastPoints[0] = new Vector3(collider.transform.position.x, collider.transform.position.y - collider.size.y / 2, collider.transform.position.z);
        raycastPoints[0] = new Vector3(collider.transform.position.x - collider.size.x / 2, collider.transform.position.y - collider.size.y / 2, collider.transform.position.z - collider.size.z / 2);
        raycastPoints[0] = new Vector3(collider.transform.position.x + collider.size.x / 2, collider.transform.position.y - collider.size.y / 2, collider.transform.position.z - collider.size.z / 2);
        raycastPoints[0] = new Vector3(collider.transform.position.x - collider.size.x / 2, collider.transform.position.y - collider.size.y / 2, collider.transform.position.z + collider.size.z / 2);
        raycastPoints[0] = new Vector3(collider.transform.position.x + collider.size.x / 2, collider.transform.position.y - collider.size.y / 2, collider.transform.position.z + collider.size.z / 2);*/

        RaycastHit hitInfo;
        var raycastHit = false;
        var distance = float.MaxValue;

        /*foreach (var point in raycastPoints)
        {
#if UNITY_EDITOR
            Debug.DrawLine(point + (Vector3.up * rayCastInset), point + (Vector3.up * rayCastInset) + (Vector3.down * GroundCheckDistance));
#endif

            if (Physics.Raycast(point + (Vector3.up * rayCastInset), Vector3.down, out hitInfo, GroundCheckDistance + rayCastInset, collisionMask))
            {
                raycastHit = true;
                distance = Mathf.Min(distance, hitInfo.distance);
            }
        }*/

        if (Physics.BoxCast(collider.center + (Vector3.up * rayCastInset), new Vector3(collider.size.x / 2, collider.size.y / 2, collider.size.z / 2),
            Vector3.down, out hitInfo, collider.transform.rotation, GroundCheckDistance + rayCastInset, collisionMask))
        {
            raycastHit = true;
            distance = hitInfo.distance;
        }

        // TODO: Correct player position based on raycast distance

        return raycastHit;
    }

    public void AttemptStateSwitch(PlayerController3D player, IPlayerState3D newState)
    {
        player.SwitchState(newState);
    }
}

