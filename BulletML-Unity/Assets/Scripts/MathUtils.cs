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
        /// <summary>
    /// Get an angle in degrees from a normalized direction clamped between 0° and 360°
    /// </summary>
    /// <param name="direction">Normalized direction</param>
    /// <returns>Angle in degrees</returns>
    public static float DirectionToAngle(Vector2 direction)
    {
        // +180° to shift the values from -180 -> 180 to 0 -> 360
        return (-Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg) + 180f;
    }
}
