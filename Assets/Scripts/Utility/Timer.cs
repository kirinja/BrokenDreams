using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float timePassed;
    private readonly float targetTime;

    public Timer(float maxTime)
    { 
        targetTime = maxTime;
        timePassed = 0f;
    }

    public bool Update(float timePassed)
    {
        this.timePassed += timePassed;
        return this.timePassed >= targetTime;
    }

    public void ResetToSurplus()
    {
        if (timePassed >= targetTime)
        {
            timePassed -= targetTime;
        }
    }

    public void Reset()
    {
        timePassed = 0f;
    }
}
