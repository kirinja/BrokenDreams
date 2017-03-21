using System;

public struct SmoothDampData
{
    private readonly float targetVelocity;
    private readonly float smoothTime;

    public SmoothDampData(float targetVelocity, float smoothTime)
    {
        if (smoothTime < MathHelper.FloatEpsilon)
        {
            throw new ArgumentOutOfRangeException("smoothTime", smoothTime, "The smooth time must be positive.");
        }

        this.targetVelocity = targetVelocity;
        this.smoothTime = smoothTime;
    }

    public float TargetVelocity
    {
        get { return targetVelocity; }
    }

    public float SmoothTime
    {
        get { return smoothTime; }
    }
}