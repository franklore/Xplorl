using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Xplorl.Grid;

public class MapFileManager
{
    private Dictionary<Vector3Int, int> indices = new Dictionary<Vector3Int, int>();

    private readonly string mapFilePath;

    private BinaryReader mapReader;

    private BinaryWriter mapWriter;

    private int nextIndex = 1;

    private int chunkBeginIndex;

    private Chunk emptyChunk;

    public MapFileManager(string mapPath)
    {
        mapFilePath = mapPath;

        emptyChunk = new Chunk(Vector3Int.zero);

        MapGenerator.CreateEmptyChunk(Vector3Int.zero, ref emptyChunk);
    }

    public void CreateMap()
    {
        Debug.Log("Create map at " + mapFilePath);
        try
        {
            using (BinaryWriter writer = new BinaryWriter(new FileStream(mapFilePath, FileMode.Create)))
            {
                
                writer.Write(0);
                writer.Write(1);
                writer.Write(0);
                writer.Write(0);
                byte[] bytes = new byte[Chunk.ClassSize - 16];
                writer.Write(bytes);
            }
        }
        catch
        {
            throw;
        }
    }

    public void LoadMap()
    {
        try
        {
            FileStream mapStream = new FileStream(mapFilePath, FileMode.Open);

            mapReader = new BinaryReader(mapStream);

            mapWriter = new BinaryWriter(mapStream);

            int indexCount = mapReader.ReadInt32();
            nextIndex = mapReader.ReadInt32();
            mapReader.ReadInt32();
            mapReader.ReadInt32();

            for (uint i = 0; i < indexCount; i++)
            {
                int x = mapReader.ReadInt32();
                int y = mapReader.ReadInt32();
                int z = mapReader.ReadInt32();
                int v = mapReader.ReadInt32();
                indices.Add(new Vector3Int(x, y, z), v);
            }

            chunkBeginIndex = (indexCount + 1) * 16 / Chunk.ClassSize + 1;
        }
        catch
        {
            throw;
        }
    }

    public void SaveMap()
    {
        Dictionary<int, Vector3Int> indicesReverse = new Dictionary<int, Vector3Int>();
        foreach (KeyValuePair<Vector3Int, int> pair in indices)
        {
            if (pair.Value > 0)
            {
                indicesReverse[pair.Value] = pair.Key;
            }
        }

        int newChunkBeginIndex = (indices.Count + 1) * 16 / Chunk.ClassSize + 1;
        for (int i = chunkBeginIndex; i < newChunkBeginIndex; i++)
        {
            if (indicesReverse.ContainsKey(i)) // not empty chunk
            {
                mapReader.BaseStream.Seek(i * Chunk.ClassSize, SeekOrigin.Begin);
                byte[] bytes = mapReader.ReadBytes(Chunk.ClassSize);
                indices[indicesReverse[i]] = nextIndex;
                WriteBytesNew(bytes);
            }
            else
            {
                mapReader.BaseStream.Seek(i * Chunk.ClassSize, SeekOrigin.Begin);
                int next = mapReader.ReadInt32();
                int prev = mapReader.ReadInt32();
                if (next * Chunk.ClassSize < mapReader.BaseStream.Length)
                {
                    mapWriter.BaseStream.Seek(next * Chunk.ClassSize, SeekOrigin.Begin);
                    mapWriter.Write(prev);
                }
                if (nextIndex != i)
                {
                    mapWriter.BaseStream.Seek(prev * Chunk.ClassSize, SeekOrigin.Begin);
                    mapWriter.Write(next);
                }
                else
                {
                    nextIndex = next;
                }
            }
        }
        
        // write indices
        mapWriter.BaseStream.Seek(0, SeekOrigin.Begin);
        mapWriter.Write(indices.Count);
        mapWriter.Write(nextIndex);
        mapWriter.Write(0);
        mapWriter.Write(0);
        foreach (KeyValuePair<Vector3Int, int> pair in indices)
        {
            mapWriter.Write(pair.Key.x);
            mapWriter.Write(pair.Key.y);
            mapWriter.Write(pair.Key.z);
            mapWriter.Write(pair.Value);
        }
        chunkBeginIndex = newChunkBeginIndex;
    }

    public int ReadChunk(Vector3Int pos, ref Chunk chunk)
    {
        int loc;
        if (indices.TryGetValue(pos, out loc))
        {
            if (loc < 0)
            {
                return 1;
            }
            chunk.Position = pos;
            mapReader.BaseStream.Seek(loc * Chunk.ClassSize, SeekOrigin.Begin);
            byte[] bytes = mapReader.ReadBytes(Chunk.ClassSize);
            chunk.ReadByteArray(bytes);
            return 0;
        }
        else
        {
            return -1;
        }
    }

    public void WriteBytesNew(byte[] bytes)
    {
        mapReader.BaseStream.Seek(nextIndex * Chunk.ClassSize, SeekOrigin.Begin);
        if (nextIndex * Chunk.ClassSize < mapReader.BaseStream.Length)
        {
            nextIndex = mapReader.ReadInt32();
            mapReader.BaseStream.Seek(-4, SeekOrigin.Current);
        }
        else
        {
            nextIndex++;
        }
        mapWriter.Write(bytes);
    }

    public void WriteChunk(Chunk chunk)
    {
        if (!chunk.Changed)
        {
            return;
        }

        int loc;

        if (chunk.isEmpty)
        {
            if (!indices.TryGetValue(chunk.Position, out loc))
            {
                indices.Add(chunk.Position, -1);
            }
            else
            {
                if (nextIndex * Chunk.ClassSize < mapWriter.BaseStream.Length)
                {
                    mapWriter.BaseStream.Seek(nextIndex * Chunk.ClassSize + 4, SeekOrigin.Begin);
                    mapWriter.Write(loc);
                }
                mapWriter.BaseStream.Seek(loc * Chunk.ClassSize, SeekOrigin.Begin);
                mapWriter.Write(nextIndex);
                nextIndex = loc;
                indices[chunk.Position] = -1;
            }
            return;
        }

        if (!indices.TryGetValue(chunk.Position, out loc))
        {
            // write chunk which doesn't exist in the file
            loc = nextIndex;
            indices.Add(chunk.Position, loc);
            WriteBytesNew(chunk.ToByteArray());
        }
        else if (loc < 0)
        {
            // write chunk which used to be empty
            loc = nextIndex;
            indices[chunk.Position] = loc;
            WriteBytesNew(chunk.ToByteArray());
        }
        else
        {
            // overwrite
            mapReader.BaseStream.Seek(loc * Chunk.ClassSize, SeekOrigin.Begin);
            mapWriter.Write(chunk.ToByteArray());
        }
        chunk.Changed = false;

    }
    public void Flush()
    {
        mapWriter.Flush();
    }

    public void Dispose()
    {
        SaveMap();
        mapWriter.Dispose();
        mapReader.Dispose();
        indices.Clear();
        nextIndex = 0;
    }
}
