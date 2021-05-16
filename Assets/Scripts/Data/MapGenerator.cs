using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xplorl.Grid;

public class MapGenerator : MonoBehaviour
{
    public static void CreateChunkOnProperty(Vector3Int position, ref Chunk chunk)
    {
        chunk.Position = position;
        for (int y = 0; y < Chunk.chunkSize; y++)
            for (int x = 0; x < Chunk.chunkSize; x++)
            {
                Vector3Int blockPosition = Chunk.CombinePosition(position, x, y);
                Vector3Int blockLayer0 = new Vector3Int(blockPosition.x, blockPosition.y, 0);
                Vector3Int blockLayer1 = new Vector3Int(blockPosition.x, blockPosition.y, 1);
                Block block = chunk[x, y];
                float v = PerlinNoise(blockLayer0, Chunk.chunkSize * 4, 3, 0.7f);
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
                        if (RandomGenerator.RandomValue(blockLayer1, 797, 3961, 78113, 1104928579) < 0.05)
                        {
                            BlockFactory.Instance.GetBlockObject(7).CreateBlock(0, ref block);
                            continue;
                        }
                    }
                    if (v > 0.5)
                    {
                        if (RandomGenerator.RandomValue(blockLayer1, 1256, 13256, 61763, 192848192) < Mathf.Clamp(v - 0.3f, 0, 0.8f))
                        {
                            BlockFactory.Instance.GetBlockObject(8).CreateBlock(0, ref block);
                            continue;
                        }
                    }
                    if (v > 0.5)
                    {
                        float r = RandomGenerator.RandomValue(blockLayer1, 917, 4930, 48481, 192948192);
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
        int freq = 1;
        float amplitude = 1;
        float maxValue = 0;
        for (int i = 0; i < octaves; i++)
        {
            total += PerlinNoise(pos, step / freq) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            freq *= 2;
        }
        return total / maxValue;
    }

    private static float PerlinNoise(Vector3Int pos, int step)
    {
        Vector3Int chunkPos, blockPos;
        Chunk.SplitPosition(pos, out chunkPos, out blockPos, step);
        Vector2 g00 = RandomGenerator.RandomVec2(chunkPos);
        Vector2 g01 = RandomGenerator.RandomVec2(chunkPos + new Vector3Int(0, 1, 0));
        Vector2 g11 = RandomGenerator.RandomVec2(chunkPos + new Vector3Int(1, 1, 0));
        Vector2 g10 = RandomGenerator.RandomVec2(chunkPos + new Vector3Int(1, 0, 0));
        Vector2 d = new Vector2((blockPos.x + 0.5f) / step, (blockPos.y + 0.5f) / step);
        Vector2 p00 = d;
        Vector2 p01 = d - new Vector2(0, 1);
        Vector2 p11 = d - new Vector2(1, 1);
        Vector2 p10 = d - new Vector2(1, 0);
        return (Interp(
            Interp(Vector2.Dot(p00, g00), Vector2.Dot(p01, g01), d.y),
            Interp(Vector2.Dot(p10, g10), Vector2.Dot(p11, g11), d.y),
            d.x) + 1) / 2;
    }

    private static float Interp(float a, float b, float t)
    {
        float tt = t * t * t * (t * (t * 6 - 15) + 10);
        return a * (1 - tt) + b * tt;
    }
}
