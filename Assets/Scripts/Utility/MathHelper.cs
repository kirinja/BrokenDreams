using UnityEngine;

public static class MathHelper
{
	public const float FloatEpsilon = 0.0001f;

	public static int Sign (float value)
	{
		return Mathf.Abs(value) > FloatEpsilon
			? value > 0.0f
			? 1
				: -1
				: 0;

	}
}