using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{

	public PlayerAttributes Attributes { get; private set; } //Kommer åt attributes överallt men kan endast ändra dem i Controller2D-klassen

	public Vector2 input;

	public Animator anim;

	public float DeaccelerationScale = 0.8f;

	public float MaxSlopeAngle = 80.0f;

	private float jumpVelocity;

	[HideInInspector]
	public float MinJumpVelocity;

	//private float maxJumpVelocity;

	public void Jump ()
	{
		Velocity.y = jumpVelocity; 
	}


	[Tooltip ("The maximum distance between the bottom of the box collider and the ground beneath it, in terms of box collider scale, for the collider to clamp to the ground.")]
	public float VerticalClampFactor = 0.25f;

	public LayerMask CollisionMask;

	public float MaxDistBetweenRays = 0.25f;

	[HideInInspector]
	public Vector2 Velocity;

	#region Constants

	public readonly float SkinWidth = 0.03f;

	private const float MinRaySpacing = 0.1f;

	public readonly KeyCode JumpKey = KeyCode.Space; 

	#endregion

	private ICharacterState characterState;

	private RaySpacing raySpacing;

	private float velocityXSmoothing;

	private float gravity;

	private BoxCollider2D boxCollider;

	public float VerticalClampDistance
	{
		get { return boxCollider.size.y * VerticalClampFactor; }
	}

	public RaycastOrigins GetUpdatedRaycastOrigins()
	{
		var skinnedBounds = SkinnedBounds;
		var raycastOrigins = new RaycastOrigins();
		raycastOrigins.BottomLeft = new Vector2(skinnedBounds.min.x, skinnedBounds.min.y);
		raycastOrigins.BottomRight = new Vector2(skinnedBounds.max.x, skinnedBounds.min.y);
		raycastOrigins.TopLeft = new Vector2(skinnedBounds.min.x, skinnedBounds.max.y);
		raycastOrigins.TopRight = new Vector2(skinnedBounds.max.x, skinnedBounds.max.y);
		return raycastOrigins;
	}

	public Vector2 CalculateVelocity (Vector2 input, float accelerationTime)
	{
		var targetVelocityX = input.x * Attributes.MaxSpeed;
		var smoothTime = accelerationTime;
		if (Mathf.Abs(input.x) < MathHelper.FloatEpsilon)
		{
			smoothTime *= DeaccelerationScale;
		}

		Velocity.x = Mathf.SmoothDamp (Velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);
		Velocity.y += gravity * Time.deltaTime;
		return Velocity * Time.deltaTime;
	}

	public CollisionData GetHorizontalCollision(Vector2 rayDirection, Vector2 rayOrigin, float rayLength)
	{
		var collisionData = new CollisionData ();
		var rayMovement = Vector2.up * raySpacing.HorizontalRaySpacing;
		for (int i = 0; i < raySpacing.HorizontalRayCount; ++i)
		{
			var hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, CollisionMask);
			if (hit)
			{
				rayLength = hit.distance;
				collisionData = new CollisionData (true, rayLength, hit.normal);
			}

			rayOrigin += rayMovement;
		}

		return collisionData;
	}

	public CollisionData GetVerticalCollision(Vector2 rayDirection, Vector2 rayOrigin, float rayLength)
	{
		var collisionData = new CollisionData();
		var rayMovement = Vector2.right * raySpacing.VerticalRaySpacing;
		for (int i = 0; i < raySpacing.VerticalRayCount; ++i)
		{
			var hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, CollisionMask);
			if (hit)
			{
				rayLength = hit.distance;
				collisionData = new CollisionData(true, rayLength, hit.normal);
			}

			rayOrigin += rayMovement;
		}

		return collisionData;
	}

	public void ResetVelocityX()
	{
		Velocity.x = velocityXSmoothing = 0.0f;
	}

	public HorizontalDirection GetHorizontalDirection(Vector2 direction)
	{
		return (HorizontalDirection)MathHelper.Sign (direction.x);
	}

	public VerticalDirection GetVerticalDirection(Vector2 direction)
	{
		return (VerticalDirection)MathHelper.Sign (direction.y);
	}

	private void Start()
	{

		Attributes = GetComponent<PlayerAttributes> ();

		anim = GetComponent<Animator> ();

		boxCollider = GetComponent<BoxCollider2D>();
		gravity = ( -2 * Attributes.MaxJumpHeight * Mathf.Pow(Attributes.MaxSpeed, 2)) / (Mathf.Pow(Attributes.MaxJumpLength / 2, 2));
		jumpVelocity = ((2 * Attributes.MaxJumpHeight * Attributes.MaxSpeed) / (Attributes.MaxJumpLength / 2));
		//gravity = -(2 * attributes.MaxJumpHeight) / Mathf.Pow (TimeToJumpApex, 2);
		//jumpVelocity = Mathf.Abs (gravity) * TimeToJumpApex;
		var positiveGravity = gravity * -1;
		//maxJumpVelocity = positiveGravity * TimeToJumpApex;
		MinJumpVelocity = Mathf.Sqrt (2 * positiveGravity * Attributes.MinJumpHeight);

		CalculateRaySpacing ();

		characterState = GetInitialCharacterState();
		characterState.Enter();
	}

	private void CalculateRaySpacing()
	{
		var distanceBetweenRays = Mathf.Clamp (MaxDistBetweenRays, MinRaySpacing, float.MaxValue);
		raySpacing = new RaySpacing (SkinnedBounds, distanceBetweenRays);
	}

	private Bounds SkinnedBounds
	{
		get
		{
			var bounds = boxCollider.bounds;
			bounds.Expand (-SkinWidth);
			return bounds;
		}
	}

	private void Update()
	{
		input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		var characterStateData = characterState.Update (input, Time.deltaTime);
		transform.Translate (characterStateData.Movement);
		if (characterStateData.NewState != null)
		{
			ChangeCharacterState (input, characterStateData);
		}
		//		GetComponent <SpriteRenderer> ().flipX = input.x > 0 ? false : true;

		Rigidbody2D rigidbody2D = GetComponent <Rigidbody2D>();
		rigidbody2D.position = transform.position;
	}

	private void ChangeCharacterState(Vector2 input, CharacterStateData characterStateData)
	{
		PrintStateSwitching (characterStateData);
		characterState.Exit ();
		characterState = characterStateData.NewState;
		characterState.Enter ();
		if (characterStateData.RunNewStateSameUpdate)
		{
			characterState.Update (input, Time.deltaTime);
		}
	}

	[Conditional ("UNITY_EDITOR")]
	private void PrintStateSwitching (CharacterStateData characterStateData)
	{
		print ("Switching from " + characterState.ToString () + "to" + characterStateData.NewState.ToString ());

	}
	private ICharacterState GetInitialCharacterState()
	{
		ICharacterState characterState = null;

		var raycastOrigins = GetUpdatedRaycastOrigins ();
		var verticalCollisionData = GetVerticalCollision (Vector2.down, raycastOrigins.BottomLeft, raySpacing.VerticalRaySpacing + SkinWidth); //Arbitrary length
		if (verticalCollisionData.Hit && verticalCollisionData.Distance < SkinWidth) {
			characterState = new GroundState (this);
		} else {
			characterState = new AirState (this, true);
		}
		return characterState;
	}

}