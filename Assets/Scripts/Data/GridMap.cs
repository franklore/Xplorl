using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap
{
    private LRUCache<Vector3Int, Chunk> chunkCache;

    private int chunkCapacity = 256;

    private MapFileManager mapFileManager;

    private bool mapLoaded = false;

    public delegate void OnSetValue(Vector3Int position);

    private event OnSetValue onSetValue;

    public delegate void CreateOnMissChunk(Vector3Int position, ref Chunk chunk);

    private CreateOnMissChunk createOnMissChunk = MapGenerator.CreateEmptyChunk;
    public Block this[Vector3Int pos]
    {
        get
        {
            Vector3Int chunkPosition, blockPosition;
            Chunk.SplitPosition(pos, out chunkPosition, out blockPosition);
            Chunk chunk;
            if (chunkCache.TryGetValue(chunkPosition, out chunk))
            {
                return chunk[blockPosition.x, blockPosition.y];
            }
            else
            {
                TryLoadChunkFromFile(chunkPosition);
                if (chunkCache.TryGetValue(chunkPosition, out chunk))
                {
                    return chunk[blockPosition.x, blockPosition.y];
                }
                else
                {
                    return default;
                }
            }
        }
        set
        {
            Block oldBlock = this[pos];
            if (oldBlock != null && !oldBlock.IsEmpty() && value.Equals(oldBlock))
            {
                return;
            }
            Vector3Int chunkPosition, blockPosition;
            Chunk.SplitPosition(pos, out chunkPosition, out blockPosition);

            Chunk chunk;
            if (!chunkCache.TryGetValue(chunkPosition, out chunk))
            {
                TryLoadChunkFromFile(chunkPosition);
                if (!chunkCache.TryGetValue(chunkPosition, out chunk))
                {
                    Chunk c = chunkCache.Fetch();
                    MapGenerator.CreateEmptyChunk(chunkPosition, ref c);
                    c[blockPosition.x, blockPosition.y].Id = value.Id;
                    c[blockPosition.x, blockPosition.y].State = value.State;
                    c[blockPosition.x, blockPosition.y].Health = value.Health;
                    chunkCache.SubmitFetch(chunkPosition);
                    c.Loaded = true;
                    onSetValue?.Invoke(pos);
                    return;
                }
            }
            chunk[blockPosition.x, blockPosition.y].Id = value.Id;
            chunk[blockPosition.x, blockPosition.y].State = value.State;
            chunk[blockPosition.x, blockPosition.y].Health = value.Health;
            onSetValue?.Invoke(pos);
        }
    }

    public Block this[int x, int y]
    {
        set => this[new Vector3Int(x, y, 0)] = value;
        get => this[new Vector3Int(x, y, 0)];
    }

    private void OnReleaseChunk(Vector3Int pos, Chunk chunk)
    {
        mapFileManager.WriteChunk(chunk);
        chunk.Loaded = false;
    }

    public GridMap()
    {
        Chunk[] chunks = new Chunk[chunkCapacity];
        for (int i = 0; i < chunkCapacity; i++)
        {
            chunks[i] = new Chunk(Vector3Int.zero);
            chunks[i].GridMap = this;
        }
        chunkCache = new LRUCache<Vector3Int, Chunk>(chunkCapacity, true, chunks);
        chunkCache.registerOnRemoveMethod(OnReleaseChunk);
    }

    public void Load(string mapFilePath, bool newMap)
    {
        if (mapLoaded)
        {
            return;
        }
        mapFileManager = new MapFileManager(mapFilePath);
        if (newMap)
        {
            mapFileManager.CreateMap();
        }
        mapFileManager.LoadMap();
        mapLoaded = true;
    }

    public void Save()
    {
        if (!mapLoaded)
        {
            return;
        }
        chunkCache.Clear();
        mapFileManager.Dispose();
        mapLoaded = false;
    }

    public void TryLoadChunkFromFile(Vector3Int position)
    {
        if (chunkCache.ContainsKey(position))
        {
            return;
        }
        Chunk chunk = chunkCache.Fetch();
        int res = mapFileManager.ReadChunk(position, ref chunk);
        if (res == -1)
        {
            createOnMissChunk.Invoke(position, ref chunk);
            chunkCache.SubmitFetch(position);
            chunk.Loaded = true;
        }
        else if (res == 1)
        {
            chunkCache.DiscardFetch();
            return;
        }
        else
        {
            chunkCache.SubmitFetch(position);
            chunk.Loaded = true;
        }
    }


    public Block GetLoadedBlock(Vector3Int position)
    {
        Vector3Int chunkPosition, blockPosition;
        Chunk.SplitPosition(position, out chunkPosition, out blockPosition);
        Chunk chunk;
        if (chunkCache.TryGetValue(chunkPosition, out chunk))
        {
            return chunk[blockPosition.x, blockPosition.y];
        }
        else
        {
            return Block.EmptyBlock;
        }
    }

    public Chunk GetChunk(Vector3Int chunkPosition)
    {
        Chunk chunk;
        if (chunkCache.TryGetValue(chunkPosition, out chunk))
        {
            return chunk;
        }
        else
        {
            TryLoadChunkFromFile(chunkPosition);
            if (chunkCache.TryGetValue(chunkPosition, out chunk))
            {
                return chunk;
            }
            else
            {
                return default;
            }
        }
    }

    public Chunk GetLoadedChunk(Vector3Int chunkPosition)
    {
        Chunk chunk;
        chunkCache.TryGetValue(chunkPosition, out chunk);
        return chunk;
    }

    public void RegisterOnSetValueMethod(OnSetValue method)
    {
        onSetValue += method;
    }

    public void RegisterCreateOnMissChunkMethod(CreateOnMissChunk method)
    {
        createOnMissChunk = method;
    }

    public void InvokeOnSetValue(Vector3Int pos)
    {
        onSetValue?.Invoke(pos);
    }
}
