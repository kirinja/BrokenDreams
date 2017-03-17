using System;
using UnityEngine;

public class RaySpacing
{
	public int HorizontalRayCount { get; private set; }
	public int VerticalRayCount { get; private set; }
	public float HorizontalRaySpacing { get; private set; }
	public float VerticalRaySpacing { get; private set; }

	public RaySpacing (Bounds bounds, float maxDistanceBetweenRays)
	{

		var boundsWidth = bounds.size.x;
		var boundsHeight = bounds.size.y;

		if (maxDistanceBetweenRays < MathHelper.FloatEpsilon)
		{
			var message = String.Format("The max distance between rays cannot be smaller than {0}.", MathHelper.FloatEpsilon);
			throw new ArgumentOutOfRangeException ("maxDistanceBetweenRays", maxDistanceBetweenRays, message);
		}

		HorizontalRayCount = Mathf.RoundToInt (boundsHeight / maxDistanceBetweenRays + 1.5f);
		VerticalRayCount = Mathf.RoundToInt(boundsWidth / maxDistanceBetweenRays + 1.5f);
		HorizontalRaySpacing = boundsHeight / (HorizontalRayCount -1);
		VerticalRaySpacing = boundsWidth / (VerticalRayCount - 1);
	}
}


