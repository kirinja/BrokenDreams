using System;
using UnityEngine;

public struct GroundState : ICharacterState
{
	private Controller2D controller;

	private RaycastOrigins raycastOrigins;

	public GroundState(Controller2D controller)
	{
		if (controller == null)
		{
			throw new ArgumentNullException("controller");
		}

		this.controller = controller;
		raycastOrigins = new RaycastOrigins();
	}

	public void Enter()
	{
		//NOTE: Empty by choice. Here for demonstration purposes.
	}

	public CharacterStateData Update(Vector2 input, float deltaTime)
	{
		if (Input.GetKeyDown (controller.JumpKey)) 
		{
			// NOTE: Since all states will detect the same input keys the simplest, albeit maybe not the most clean way, to
			// left the air state handle to jump.
			return new CharacterStateData (Vector2.zero, new AirState (controller), true);
		}

		raycastOrigins = controller.GetUpdatedRaycastOrigins ();
		var velocity = controller.CalculateVelocity (input, controller.Attributes.GroundAccelerationTime);
		controller.Velocity.y = velocity.y = 0.0f; // NOTE: Gravity will keep building a negative y velocity so we need to keep it in check by setting it to zero.

		var movement = Vector2.zero;
		var airborne = HandleMovement(velocity, ref movement);
		var characterStateData = GetCharacterStateData (movement, airborne);
//		controller.anim.SetFloat("speed", Math.Abs(velocity.x));
		return characterStateData;
	}

	private CharacterStateData GetCharacterStateData(Vector2 movement, bool airborne)
	{
		var characterStateData = new CharacterStateData ();
		characterStateData.Movement = movement;
		if (airborne)
		{
			characterStateData.NewState = new AirState (controller, true);
		}
		return characterStateData;
	}


	private bool HandleMovement(Vector2 velocity, ref Vector2 movement)
	{
		var direction = GetMovementDirection(velocity);
		var length = velocity.magnitude + controller.SkinWidth;

		MoveAlongGround (direction, length, ref movement);
		var airborne = AdjustVerticalMovement (ref movement);
		return airborne;
	}

	private Vector2 GetMovementDirection (Vector2 velocity)
	{
		var totalVelocityLength = velocity.magnitude + controller.SkinWidth;
		var verticalCollisionData = controller.GetVerticalCollision (Vector2.down, raycastOrigins.BottomLeft, totalVelocityLength); //NOTE: Don't care too much about the length as long as it's at least as long as needed.
		var horizontalDirection = controller.GetHorizontalDirection(velocity);

		Vector2 direction;
		var isPlanar = Vector2.Angle(Vector2.up, verticalCollisionData.SurfaceNormal) < MathHelper.FloatEpsilon;
		if (isPlanar)
		{
			controller.Velocity.y = velocity.y = 0.0f;
			direction = velocity.normalized;
		}
		else
		{
			direction = GetSlopeDirection (horizontalDirection, verticalCollisionData.SurfaceNormal);
		}

		return direction;
	}

	private Vector2 GetSlopeDirection(HorizontalDirection horizontalDirection, Vector2 surfaceNormal)
	{
		var rotationAngle = 90.0f * -horizontalDirection.ToInt ();
		return Quaternion.Euler (0.0f, 0.0f, rotationAngle) * surfaceNormal;
	}

	public void Exit()
	{
		// NOTE: Empty by choice. Here for demonstration purposes.
	}

	private void MoveAlongGround(Vector2 direction, float length, ref Vector2 movement)
	{
		var updatedRayCastOrigins = raycastOrigins.Move (movement);
		var horizontalDirection = controller.GetHorizontalDirection (direction);
		var sideRayOrigin = horizontalDirection == HorizontalDirection.Right ? updatedRayCastOrigins.BottomRight : updatedRayCastOrigins.BottomLeft;
		var horizontalCollisionData = controller.GetHorizontalCollision (direction, sideRayOrigin, length);
		var verticalCollisionData = GetVerticalCollisionData (direction, length, updatedRayCastOrigins);

		var noHit = !(horizontalCollisionData.Hit || verticalCollisionData.Hit);
		if (noHit)
		{
			movement += direction * (length - controller.SkinWidth);
		}
		else
		{
			// NOTE: Given that the horizontal and vertical rays share a common origin the code below may switch between
			// saying that the vertical hit is closer than the horizontal hit and vice versa. In the end it doesn't matter
			// given that the movement direction for both scenarios are the same; verticalCollisionData.Distance IS equal
			// to horizontalCollisionData, less any floating point imprecisions
			var isAboveClosestHit = !horizontalCollisionData.Hit || (verticalCollisionData.Hit && verticalCollisionData.Distance < horizontalCollisionData.Distance);
			if (isAboveClosestHit)
			{
				movement += direction * (verticalCollisionData.Distance - controller.SkinWidth);
			}
			else 
			{
				var slopeAngle = Vector2.Angle(Vector2.up, horizontalCollisionData.SurfaceNormal);
				if (slopeAngle < controller.MaxSlopeAngle)
				{
					// Move to start of slope.
					var moveLength = horizontalCollisionData.Distance - controller.SkinWidth;
					movement += direction * moveLength;

					// Calculate remaining movement length and continue along the new slope.
					length -= moveLength;
					direction = GetSlopeDirection(horizontalDirection, horizontalCollisionData.SurfaceNormal);

					MoveAlongGround(direction, length, ref movement);
				}
				else
				{
					movement += direction * (horizontalCollisionData.Distance - controller.SkinWidth);
				}
			}
		}
	}

	private CollisionData GetVerticalCollisionData(Vector2 direction, float length, RaycastOrigins updatedRayCastOrigins)
	{
		var verticalCollisionData = new CollisionData();
		if (direction.y > MathHelper.FloatEpsilon)
		{
			var aboveRayOrigin = updatedRayCastOrigins.TopLeft;
			verticalCollisionData = controller.GetVerticalCollision (direction, aboveRayOrigin, length);
		}
		else if (direction.y < -MathHelper.FloatEpsilon)
		{
			var belowRayOrigin = updatedRayCastOrigins.BottomLeft;
			verticalCollisionData = controller.GetVerticalCollision (direction, belowRayOrigin, length);
		}

		return verticalCollisionData;
	}

	private bool AdjustVerticalMovement(ref Vector2 movement)
	{
		var airborne = false;
		var updatedRayCastOrigins = raycastOrigins.Move (movement);
		var verticalCollisionData = controller.GetVerticalCollision (Vector2.down, updatedRayCastOrigins.BottomLeft, controller.VerticalClampDistance);
		if (verticalCollisionData.Hit) {
			// NOTE: This is cheating. It's "ok" for reasonable level design and character speed, so leaving it "as is" for now.
			// The proper way to do this would be to figure out the angle of the slope below and do vector / trigonometry math
			// to get the intersection between the last direction of the character and the slope angle and backtrack to that point.
			// This needs to be done recursively since there can be multiple different slope angles between the point where the character
			//  went airborne and the contact point below the current position of the character (the calculated position that is).
			movement.y -= verticalCollisionData.Distance - controller.SkinWidth;
			controller.Velocity.y = 0.0f;
		} else {
			// NOTE: A solution that would look more pleasing would be to change the y velocity to something positive to give the character
			// a feeling of flying in an arc move.
			airborne = true;
		}
		return airborne;
	}
}