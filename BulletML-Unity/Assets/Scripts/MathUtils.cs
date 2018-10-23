using UnityEngine;

public static class MathUtils
{
    /// <summary>
    /// Get a normalized direction from an angle in degrees
    /// </summary>
    /// <param name="angle">Angle in degrees</param>
    /// <returns></returns>
    public static UnityEngine.Vector2 AngleToDirection(float angle)
    {
        // Convert the angle in radians
        angle = (angle + 90f) * Mathf.Deg2Rad;

        var direction = new UnityEngine.Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        direction.Normalize(); // make sure the direction is normalized

        return direction;
    }
}
