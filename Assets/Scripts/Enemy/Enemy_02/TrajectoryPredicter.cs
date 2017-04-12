using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TrajectoryPredicter {

    public static Vector3 HitTargetBySpeed(Vector3 startPosition, Vector3 targetPosition, Vector3 gravityBase, float launchSpeed)
    {
        Vector3 AtoB = targetPosition - startPosition;
        Vector3 horizontal = GetHorizontalVector(AtoB, gravityBase);
        float horizontalDistance = horizontal.magnitude;
        Vector3 vertical = GetVerticalVector(AtoB, gravityBase);
        float verticalDistance = vertical.magnitude * Mathf.Sign(Vector3.Dot(vertical, -gravityBase));

        float x2 = horizontalDistance * horizontalDistance;
        float v2 = launchSpeed * launchSpeed;
        float v4 = launchSpeed * launchSpeed * launchSpeed * launchSpeed;

        float gravMag = gravityBase.magnitude;

        float launchTest = v4 - (gravMag * ((gravMag * x2) + (2 * verticalDistance * v2)));

        Vector3[] launch = new Vector3[2];

        if (launchTest < 0)
        {
            launch[0] = (horizontal.normalized * launchSpeed * Mathf.Cos(45.0f * Mathf.Deg2Rad)) - (gravityBase.normalized * launchSpeed * Mathf.Sin(45.0f * Mathf.Deg2Rad));
            launch[1] = (horizontal.normalized * launchSpeed * Mathf.Cos(45.0f * Mathf.Deg2Rad)) - (gravityBase.normalized * launchSpeed * Mathf.Sin(45.0f * Mathf.Deg2Rad));
        }
        else
        {
            float[] tanAngle = new float[2];
            tanAngle[0] = (v2 - Mathf.Sqrt(v4 - gravMag * ((gravMag * x2) + (2 * verticalDistance * v2)))) / (gravMag * horizontalDistance);
            tanAngle[1] = (v2 + Mathf.Sqrt(v4 - gravMag * ((gravMag * x2) + (2 * verticalDistance * v2)))) / (gravMag * horizontalDistance);

            float[] finalAngle = new float[2];
            finalAngle[0] = Mathf.Atan(tanAngle[0]);
            finalAngle[1] = Mathf.Atan(tanAngle[1]);
            launch[0] = (horizontal.normalized * launchSpeed * Mathf.Cos(finalAngle[0])) - (gravityBase.normalized * launchSpeed * Mathf.Sin(finalAngle[0]));
            launch[1] = (horizontal.normalized * launchSpeed * Mathf.Cos(finalAngle[1])) - (gravityBase.normalized * launchSpeed * Mathf.Sin(finalAngle[1]));
        }

        return launch[0];
    }
    public static Vector3 GetHorizontalVector(Vector3 AtoB, Vector3 gravityBase)
    {
        Vector3 output;
        Vector3 perpendicular = Vector3.Cross(AtoB, gravityBase);
        perpendicular = Vector3.Cross(gravityBase, perpendicular);
        output = Vector3.Project(AtoB, perpendicular);
        return output;
    }

    public static Vector3 GetVerticalVector(Vector3 AtoB, Vector3 gravityBase)
    {
        Vector3 output;
        output = Vector3.Project(AtoB, gravityBase);
        return output;
    }

}
