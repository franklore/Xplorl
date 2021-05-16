using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Xplorl.Grid;

public class MapFileManager
{
    private Dictionary<Vector3Int, int> indices = new Dictionary<Vector3Int, int>();

    private readonly string mapName;

    private readonly string mapRootDir;

    private readonly string mapFilePath;

    private readonly string indexFilePath;

    private BinaryReader mapReader;

    private BinaryWriter mapWriter;

    private int indicesCounter = 0;

    private Chunk emptyChunk;

    public MapFileManager(string mapName)
    {
        this.mapName = mapName;

        mapRootDir = Application.streamingAssetsPath + "/map/" + mapName;

        mapFilePath = Path.Combine(mapRootDir, "map.dat");

        indexFilePath = Path.Combine(mapRootDir, "map_idx.dat");

        emptyChunk = new Chunk(Vector3Int.zero);

        MapGenerator.CreateEmptyChunk(Vector3Int.zero, ref emptyChunk);
    }

    public void CreateMap()
    {
        if (!Directory.Exists(mapRootDir)) {
            Directory.CreateDirectory(mapRootDir);
        }
        try
        {
            using (BinaryWriter writer = new BinaryWriter(new FileStream(mapFilePath, FileMode.Create)))
            {

            }
            using (BinaryWriter writer = new BinaryWriter(new FileStream(indexFilePath, FileMode.Create)))
            {

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

            using (BinaryReader reader = new BinaryReader(new FileStream(indexFilePath, FileMode.Open)))
            {
                //indices.Clear();
                //indicesCounter = 0;
                //int chunkPos = 0;
                //while (reader.BaseStream.Position < reader.BaseStream.Length)
                //{
                //    int x = reader.ReadInt32();
                //    int y = reader.ReadInt32();
                //    int z = reader.ReadInt32();
                //    indices.Add(new Vector3Int(x, y, z), chunkPos++);
                //    indicesCounter++;
                //}
                int maxIndex = 0;
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int x = reader.ReadInt32();
                    int y = reader.ReadInt32();
                    int z = reader.ReadInt32();
                    int v = reader.ReadInt32();
                    indices.Add(new Vector3Int(x, y, z), v);
                    if (v > maxIndex)
                    {
                        maxIndex = v;
                    }
                    indicesCounter = maxIndex + 1;
                }
            }
        }
        catch
        {
            throw;
        }
    }

    public void SaveMap()
    {
        using (BinaryWriter writer = new BinaryWriter(new FileStream(indexFilePath, FileMode.Open)))
        {
            foreach (KeyValuePair<Vector3Int, int> pair in indices)
            {
                writer.Write(pair.Key.x);
                writer.Write(pair.Key.y);
                writer.Write(pair.Key.z);
                writer.Write(pair.Value);
            }
        }
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

    public void WriteChunk(Chunk chunk)
    {
        if (!chunk.Changed)
        {
            return;
        }

        if (chunk.isEmpty)
        {
            if (!indices.ContainsKey(chunk.Position))
            {
                indices.Add(chunk.Position, -1);
            }
            else
            {
                indices[chunk.Position] = -1;
            }
            return;
        }

        int loc;
        if (!indices.TryGetValue(chunk.Position, out loc))
        {
            loc = indicesCounter++;
            indices.Add(chunk.Position, loc);
        }
        else if (loc < 0)
        {
            loc = indicesCounter++;
            indices[chunk.Position] = loc;
        }

        chunk.Changed = false;
        mapReader.BaseStream.Seek(loc * Chunk.ClassSize, SeekOrigin.Begin);
        mapWriter.Write(chunk.ToByteArray());
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
        indicesCounter = 0;
    }
}
