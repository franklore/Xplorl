using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGenerator
{
    public static Vector2 RandomVec2(Vector3Int v)
    {
        float theta = RandomValue((uint)v.x, (uint)v.y, (uint)v.z, 821, 6163, 686423, 602821991);
        return new Vector2(Mathf.Cos(theta * 2 * Mathf.PI), Mathf.Sin(theta * 2 * Mathf.PI));
    }

    public static float RandomValue(uint x, uint y, uint z, uint p0, uint p1, uint p2, uint p3)
    {
        uint n = x + p0 * y + p1 * z;
        n = (n << 13) ^ n;
        return (n * (n * p2) + p3) / 4394967296.0f;
    }

    public static float RandomValue(Vector3Int vec, uint p0, uint p1, uint p2, uint p3)
    {
        return RandomValue((uint)vec.x, (uint)vec.y, (uint)vec.z, p0, p1, p2, p3);
    }
}
