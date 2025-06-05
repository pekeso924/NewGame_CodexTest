using UnityEngine;

public static class ReflectionUtility
{
    public static Vector3 Reflect(Vector3 direction, Vector3 normal)
    {
        return direction - 2f * Vector3.Dot(direction, normal) * normal;
    }
}
