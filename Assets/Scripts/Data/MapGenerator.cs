using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xplorl.Grid;

public class MapGenerator : MonoBehaviour
{
    public static void CreateChunkOnProperty(Vector3Int position, ref Chunk chunk)
    {
        uint randomSeed = BlockMap.Instance.mapData.randomSeed;
        chunk.Position = position;
        for (int y = 0; y < Chunk.chunkSize; y++)
            for (int x = 0; x < Chunk.chunkSize; x++)
            {
                Vector3Int blockPosition = Chunk.CombinePosition(position, x, y);
                Vector3Int blockLayer0 = new Vector3Int(blockPosition.x, blockPosition.y, 0);
                Vector3Int blockLayer1 = new Vector3Int(blockPosition.x, blockPosition.y, 1);
                Block block = chunk[x, y];
                float v = PerlinNoise(blockLayer0, 6, 3, 0.7f) + (1 / (blockPosition.magnitude + 1));
                if (position.z == 0)
                {
                    if (v > 0.5)
                    {
                        BlockFactory.Instance.GetBlockObject(2).CreateBlock(0, ref block);
                    }
                    else
                    {
                        BlockFactory.Instance.GetBlockObject(3).CreateBlock(0, ref block);
                    }
                }
                else if (position.z == 1)
                {
                    if (v > 0.55)
                    {
                        if (RandomGenerator.RandomValue(blockLayer1, randomSeed + 1) < 0.05)
                        {
                            BlockFactory.Instance.GetBlockObject(7).CreateBlock(0, ref block);
                            continue;
                        }
                    }
                    if (v > 0.5)
                    {
                        if (RandomGenerator.RandomValue(blockLayer1, randomSeed + 2) < Mathf.Clamp(v - 0.3f, 0, 0.8f))
                        {
                            BlockFactory.Instance.GetBlockObject(8).CreateBlock(0, ref block);
                            continue;
                        }
                    }
                    if (v > 0.5)
                    {
                        float r = RandomGenerator.RandomValue(blockLayer1, randomSeed + 3);
                        if (r < 0.05)
                        {
                            BlockFactory.Instance.GetBlockObject(9).CreateBlock(0, ref block);
                            continue;
                        }
                        else if (r < 0.1)
                        {
                            BlockFactory.Instance.GetBlockObject(4).CreateBlock(0, ref block);
                            continue;
                        }
                    }
                    block.SetEmpty();
                }

            }
    }

    public static void CreateEmptyChunk(Vector3Int position, ref Chunk chunk)
    {
        //Chunk chunk = new Chunk(position);
        chunk.Position = position;
        for (int y = 0; y < Chunk.chunkSize; y++)
            for (int x = 0; x < Chunk.chunkSize; x++)
            {
                chunk[x, y].SetEmpty();
            }
        //return chunk;
    }

    private static float PerlinNoise(Vector3Int pos, int step, int octaves, float persistence)
    {
        float total = 0;
        float amplitude = 1;
        float maxValue = 0;
        for (int i = 0; i < octaves; i++)
        {
            total += PerlinNoise(pos, step - i) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
        }
        return total / maxValue;
    }

    private static float PerlinNoise(Vector3Int pos, int step)
    {
        UnityEngine.Profiling.Profiler.BeginSample("Perlin");
        uint randomSeed = BlockMap.Instance.mapData.randomSeed;
        Vector3Int chunkPos, blockPos;
        Chunk.SplitPosition(pos, out chunkPos, out blockPos, step);
        Vector2 g00 = RandomGenerator.RandomVec2(chunkPos, randomSeed);
        Vector2 g01 = RandomGenerator.RandomVec2(chunkPos + new Vector3Int(0, 1, 0), randomSeed);
        Vector2 g11 = RandomGenerator.RandomVec2(chunkPos + new Vector3Int(1, 1, 0), randomSeed);
        Vector2 g10 = RandomGenerator.RandomVec2(chunkPos + new Vector3Int(1, 0, 0), randomSeed);
        Vector2 d = new Vector2((blockPos.x + 0.5f) / (1 << step), (blockPos.y + 0.5f) / (1 << step));
        Vector2 p00 = d;
        Vector2 p01 = d - new Vector2(0, 1);
        Vector2 p11 = d - new Vector2(1, 1);
        Vector2 p10 = d - new Vector2(1, 0);
        float v = (Interp(
            Interp(Vector2.Dot(p00, g00), Vector2.Dot(p01, g01), d.y),
            Interp(Vector2.Dot(p10, g10), Vector2.Dot(p11, g11), d.y),
            d.x) + 1) / 2;
        UnityEngine.Profiling.Profiler.EndSample();

        return v;
    }

    private static float Interp(float a, float b, float t)
    {
        float tt = t * t * t * (t * (t * 6 - 15) + 10);
        return a * (1 - tt) + b * tt;
    }
}
