using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xplorl.Grid;

public class Chunk
{
    public static int chunkSize = 16;

    public static int ClassSize { get => chunkSize * chunkSize * Block.ClassSize; }

    //public static int compressedSize = chunkSize * chunkSize * Block.compressedSize;

    private Block[,] blocks = new Block[chunkSize, chunkSize];

    private Vector3Int position;
    public Vector3Int Position { get => position; set => position = value; }

    private bool changed = false;
    public bool Changed { get => changed; set => changed = value; }

    private bool loaded = false;

    public bool Loaded { get => loaded; set => loaded = value; }

    private int validCount = 0;

    public bool isEmpty
    {
        get => validCount == 0;
    }

    private GridMap gridMap;

    public GridMap GridMap { get => gridMap; set => gridMap = value; }

    public void UpdateValue(int x, int y, int validChange)
    {
        if (loaded)
        {
            gridMap?.InvokeOnSetValue(CombinePosition(Position, x, y));
        }
        validCount += validChange;
        changed = true;
    }

    public Block this[int x, int y]
    {
        get => blocks[x, y];
    }

    public Chunk(Vector3Int position)
    {
        this.position = position;
        for (int y = 0; y < chunkSize; y++)
            for (int x = 0; x < chunkSize; x++)
            {
                blocks[x, y] = new Block();
                blocks[x, y].Chunk = this;
                blocks[x, y].BlockPosition = new Vector3Int(x, y, 0);
            }
    }

    public void ReadByteArray(byte[] bytes)
    {
        ReadByteArray(bytes, 0);
    }

    public void ReadByteArray(byte[] bytes, int start)
    {
        try
        {
            for (int col = 0; col < chunkSize; col++)
            {
                for (int row = 0; row < chunkSize; row++)
                {
                    this[row, col].FromByteArray(bytes, start + (row + col * chunkSize) * Block.ClassSize);
                    if (!this[row, col].IsEmpty())
                    {
                        validCount++;
                    }
                }
            }
        }
        catch
        {
            throw;
        }
    }

    public void FillByteArray(ref byte[] bytes, int offset)
    {
        try
        {
            for (int col = 0; col < chunkSize; col++)
            {
                for (int row = 0; row < chunkSize; row++)
                {
                    this[row, col].FillByteArray(ref bytes, offset + (row + col * chunkSize) * Block.ClassSize);
                }
            }
        }
        catch
        {
            throw;
        }
    }

    public byte[] ToByteArray()
    {
        byte[] bytes = new byte[ClassSize];
        FillByteArray(ref bytes, 0);
        return bytes;
    }

    public Vector3Int[] BlockPositionArray()
    {
        Vector3Int[] vectors = new Vector3Int[chunkSize * chunkSize];
        for (int col = 0; col < chunkSize; col++)
        {
            for (int row = 0; row < chunkSize; row++)
            {
                vectors[col * chunkSize + row] = new Vector3Int(Position.x * chunkSize + row, Position.y * chunkSize + col, Position.z);
            }
        }
        return vectors;
    }

    public static void SplitPosition(Vector3Int position, out Vector3Int chunkPosition, out Vector3Int blockPosition)
    {
        SplitPosition(position, out chunkPosition, out blockPosition, chunkSize);
    }

    public static void SplitPosition(Vector3Int position, out Vector3Int chunkPosition, out Vector3Int blockPosition, int chunkSize)
    {
        chunkPosition = new Vector3Int(0, 0, position.z);
        blockPosition = new Vector3Int(0, 0, position.z);
        if (position.x % chunkSize >= 0)
        {
            blockPosition.x = position.x % chunkSize;
            chunkPosition.x = position.x / chunkSize;
        }
        else
        {
            blockPosition.x = position.x % chunkSize + chunkSize;
            chunkPosition.x = position.x / chunkSize - 1;
        }
        if (position.y % chunkSize >= 0)
        {
            blockPosition.y = position.y % chunkSize;
            chunkPosition.y = position.y / chunkSize;
        }
        else
        {
            blockPosition.y = position.y % chunkSize + chunkSize;
            chunkPosition.y = position.y / chunkSize - 1;
        }
    }

    public static Vector3Int CombinePosition(Vector3Int chunkPosition, int x, int y)
    {
        return new Vector3Int(chunkPosition.x * chunkSize + x, chunkPosition.y * chunkSize + y, chunkPosition.z);
    }

    public static Vector3Int CombinePosition(Vector3Int chunkPosition, Vector3Int blockPosition)
    {
        return CombinePosition(chunkPosition, blockPosition.x, blockPosition.y);
    }

}
