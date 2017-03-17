using UnityEngine;

public struct RaycastOrigins
{
	public Vector2 TopLeft;
	public Vector2 TopRight;
	public Vector2 BottomLeft;
	public Vector2 BottomRight;

	public RaycastOrigins Move(Vector2 movement)
	{
		var copy = new RaycastOrigins ();
		copy.TopLeft = TopLeft + movement;
		copy.TopRight = TopRight + movement;
		copy.BottomLeft = BottomLeft + movement;
		copy.BottomRight = BottomRight + movement;
		return copy;

	}
}