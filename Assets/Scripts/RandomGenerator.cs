using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGenerator
{


    public static Vector2 RandomVec2(Vector3Int v, uint seed)
    {
        float theta = RandomValue(v, seed);
        return new Vector2(Mathf.Cos(theta * 2 * Mathf.PI), Mathf.Sin(theta * 2 * Mathf.PI));
    }

    public static float RandomValue(Vector3Int vec, uint seed)
    {
         return RandomValue((uint)vec.x, (uint)vec.y, (uint)vec.z + seed, 821, 6163, 686423, 602821991);
    }

    public static float RandomValue(uint x, uint y, uint z, uint p0, uint p1, uint p2, uint p3)
    {
        uint n = x + p0 * y + p1 * z;
        n = (n << 13) ^ n;
        return (n * (n * p2) + p3) / 4394967296.0f;
    }
}
