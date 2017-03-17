using System;
using UnityEngine;

public struct AirState : ICharacterState
{
	private Controller2D controller;

	private RaycastOrigins raycastOrigins;

	private int jumpCount;

	public AirState (Controller2D controller, bool fell = false)
	{
		if (controller == null)
		{
			throw new ArgumentNullException("controller");
		}

		this.controller = controller;
		raycastOrigins = new RaycastOrigins();
		jumpCount = fell ? 1 : 0;
	}

	public void Enter()
	{
//		controller.anim.SetBool("inAir", true);
	}

	public CharacterStateData Update(Vector2 input, float deltaTime)
	{
		raycastOrigins = controller.GetUpdatedRaycastOrigins();
		if (Input.GetKeyDown(controller.JumpKey))
		{
			if (jumpCount < 1)
			{
				Jump();
			}
			else if (Input.GetKeyUp(controller.JumpKey) && controller.Velocity.y > controller.MinJumpVelocity)
			{
				controller.Velocity.y = controller.MinJumpVelocity;
			}

		}
		var velocity = controller.CalculateVelocity(input, controller.Attributes.AirAccelerationTime);
		return HandleMovement (velocity, input, deltaTime);

	}

	private void Jump()
	{
		++jumpCount;
		controller.Jump();
	}


	public void Exit()
	{
//		controller.anim.SetBool("inAir", false);
	}

	private void HandleHorizontalMovement(Vector2 velocity, ref CharacterStateData characterStateData)
	{
		var horizontalDirection = controller.GetHorizontalDirection (velocity);
		if (horizontalDirection != HorizontalDirection.None) 
		{
			var rayDirection = Vector2.right * horizontalDirection.ToInt ();
			var rayOrigin = horizontalDirection == HorizontalDirection.Right ? raycastOrigins.BottomRight : raycastOrigins.BottomLeft;
			var rayLength = Mathf.Abs (velocity.x) + controller.SkinWidth;
			var collisionData = controller.GetHorizontalCollision (rayDirection, rayOrigin, rayLength);
			if (collisionData.Hit) 
			{
				characterStateData.Movement.x += horizontalDirection.ToInt () * (collisionData.Distance - controller.SkinWidth);
				controller.ResetVelocityX ();
			}
			else 
			{
				characterStateData.Movement.x += horizontalDirection.ToInt () * (rayLength - controller.SkinWidth);
			}
		}
	}

	private CharacterStateData HandleMovement(Vector2 velocity, Vector2 input, float deltaTime)
	{
		var characterStateData = new CharacterStateData();
		HandleHorizontalMovement (velocity, ref characterStateData);
		HandleVerticalMovement (deltaTime, velocity, ref characterStateData);

		return characterStateData;
	}

	private void HandleVerticalMovement(float deltaTime, Vector2 velocity, ref CharacterStateData characterStateData)
	{
		var updatedRayCastOrigins = raycastOrigins.Move(characterStateData.Movement);
		var verticalDirection = controller.GetVerticalDirection(velocity);
		var rayLength = Mathf.Abs(velocity.y) + controller.SkinWidth;
		//var rayLength = (isWallSliding ? controller.WallSlideSpeed * deltaTime: Mathf.Abs(velocity.y)) + controller.SkinWidth;
		var rayDirection = Vector2.up * verticalDirection.ToInt();
		var rayOrigin = verticalDirection == VerticalDirection.Up ? updatedRayCastOrigins.TopLeft : updatedRayCastOrigins.BottomLeft;
		var collisionData = controller.GetVerticalCollision(rayDirection, rayOrigin, rayLength);
		if (collisionData.Hit)
		{
			characterStateData.Movement.y += verticalDirection.ToInt() * (collisionData.Distance - controller.SkinWidth);
			if (velocity.y < 0.0f)
			{
				characterStateData.NewState = new GroundState(controller);
			}

			controller.Velocity.y = 0.0f;
		}
		else
		{
			var yVelocity = verticalDirection.ToInt() * (rayLength - controller.SkinWidth);
			characterStateData.Movement.y += yVelocity;
		}
	}

}
