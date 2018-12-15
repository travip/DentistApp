using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LpmsAPI
{
    public static Quaternion GetOrientation(LpmsConnection conn)
    {
        return conn.sensorOrientation;
    }

    public static void ResetOrientation(LpmsConnection conn)
    {
        conn.SetOrientationOffset(0);
    }
}
