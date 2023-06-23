using UnityEngine;


public static class VectorUtils {

    /// <summary>
    /// Returns a normalized vector at a point around a sphere
    /// </summary>

    public static Vector3 NormalizedSphericalPosition(float radius, float angle, float elevation) {
        float x = radius * Mathf.Cos(angle) * Mathf.Sin(elevation);
        float y = radius * Mathf.Sin(angle) * Mathf.Sin(elevation);
        float z = radius * Mathf.Cos(elevation);
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Wrap a vector between low and high
    /// </summary>
    public static Vector2 Wrap(Vector2 vec, float rangeLow = -1, float rangeHigh = 1) {
        Vector2 wrappedVec = vec;
        wrappedVec.x = WrapValue(wrappedVec.x, rangeLow, rangeHigh);
        wrappedVec.y = WrapValue(wrappedVec.y, rangeLow, rangeHigh);
        return wrappedVec;
    }

    public static float WrapValue(float value, float rangeLow, float rangeHigh) {
        float range = rangeHigh - rangeLow;
        float wrappedValue = value;
        if(wrappedValue < rangeLow) {
            wrappedValue += range;
        } else if (wrappedValue > rangeHigh) {
            wrappedValue -= range;
        }
        return wrappedValue;
    }
}