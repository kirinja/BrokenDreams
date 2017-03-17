using System;
using UnityEngine;

public struct CollisionData
{
	public bool Hit { get; private set; }
	public float Distance { get; private set; }
	public Vector2 SurfaceNormal { get; private set; }

	public CollisionData (bool hit, float distance, Vector2 surfaceNormal)
	{
		if (distance < 0.0f)
		{
			throw new ArgumentOutOfRangeException ("distance", distance, "Distance cannot be negative.");
		}

		Hit = hit;
		Distance = distance;
		SurfaceNormal = surfaceNormal;

	}
}