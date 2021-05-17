using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Xplorl.Grid;

public class BlockMap : MonoBehaviour
{
    private LRUCache<Vector3Int, RenderedChunk> chunkCache;

    private Queue<Vector3Int> loadQueue;

    private GridMap gridMap;

    private int chunkCapacity = 32;

    public int spriteSize = 16;

    private bool mapLoaded = false;

    private Vector3Int observerChunkPosition = new Vector3Int(0, 0, 0);

    public static int layerTop = 1;

    public static int layerBottom = 0;

    public static int propertyLayer = layerTop + 1;

    public UnityEngine.UI.Image minimap;

    public UnityEngine.UI.Text debugText;

    public Color[] colors = new Color[6] { Color.clear, Color.white, Color.green, Color.blue, Color.gray, Color.black };

    private static BlockMap instance;

    public static BlockMap Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindWithTag("PropertyMap").GetComponent<BlockMap>();
            }
            return instance;
        }
    }

    private void OnReleaseChunk(Vector3Int pos, RenderedChunk renderedChunk)
    {
        renderedChunk.Clear();
        Debug.Log("Release " + pos + ", " + renderedChunk);
    }

    private IEnumerator ChunkLoaderCoroutine()
    {
        while (true)
        {
            bool drawSuccess = false;
            while (!drawSuccess && loadQueue.Count > 0)
            {
                Vector3Int vec = loadQueue.Dequeue();
                drawSuccess = DrawChunk(vec);         
            }
            yield return null;
        }
    }

    private void Start()
    {
        loadQueue = new Queue<Vector3Int>();

        gridMap = new GridMap();
        gridMap.RegisterCreateOnMissChunkMethod(CreateOnMissChunk);
        gridMap.RegisterOnSetValueMethod(OnGridmapSetValue);

        StartCoroutine(ChunkLoaderCoroutine());

        Texture2D minimapTexture = new Texture2D(144, 144);
        minimap.sprite = Sprite.Create(minimapTexture, new Rect(0, 0, 144, 144), new Vector2(0.5f, 0.5f));

        Color[] emptyColors = new Color[spriteSize * Chunk.chunkSize * spriteSize * Chunk.chunkSize];
        for (int i = 0; i < spriteSize * Chunk.chunkSize * spriteSize * Chunk.chunkSize; i++)
        {
            emptyColors[i] = Color.clear;
        }

        RenderedChunk[] renderedChunks = new RenderedChunk[chunkCapacity];
        for (int i = 0; i < chunkCapacity; i++)
        {
            GameObject go = new GameObject("Chunk" + i);
            go.transform.parent = transform;
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            Texture2D texture = new Texture2D(spriteSize * Chunk.chunkSize, spriteSize * Chunk.chunkSize);
            texture.SetPixels(emptyColors);
            texture.Apply();
            texture.filterMode = FilterMode.Point;
            renderer.sprite = Sprite.Create(texture, new Rect(0, 0, spriteSize * Chunk.chunkSize, spriteSize * Chunk.chunkSize), new Vector2(0, 0), spriteSize);
            renderedChunks[i] = go.AddComponent<RenderedChunk>();
            renderer.gameObject.SetActive(false);
        }

        chunkCache = new LRUCache<Vector3Int, RenderedChunk>(chunkCapacity, true, renderedChunks);
        chunkCache.registerOnRemoveMethod(OnReleaseChunk);

        Load(SceneData.Instance.NewMap);
    }

    private void Update()
    {
        Vector3 observer = Camera.main.transform.position;
        Vector3Int chunkPosition, blockPosition;
        Chunk.SplitPosition(Vector3Int.FloorToInt(observer), out chunkPosition, out blockPosition);
        setObserverPosition(chunkPosition);
        minimap.rectTransform.localPosition = new Vector3(8, 8, 0) - blockPosition;
        SetDebugInfo();
    }

    private void SetDebugInfo()
    {
        string debug = "";
        Vector3Int chunkPosition, blockPosition;
        Vector3Int mousePosition = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3Int position = new Vector3Int(mousePosition.x, mousePosition.y, 0);
        Chunk.SplitPosition(mousePosition, out chunkPosition, out blockPosition);

        Block topBlock = null;
        if (mapLoaded)
        {
            for (int i = layerTop - 1; i >= 0; i--)
            {
                topBlock = gridMap[new Vector3Int(position.x, position.y, i)];
                if (topBlock != null && !topBlock.IsEmpty())
                {
                    break;
                }
            }
            if (topBlock != null)
            {
                debug += "block: id = " + topBlock.Id + " state = " + topBlock.State + "\n";
                debug += "properties int: " + topBlock.getPropertyAsInt(0) + " | " +
                    topBlock.getPropertyAsInt(1) + " | " +
                    topBlock.getPropertyAsInt(2) + " | " +
                    topBlock.getPropertyAsInt(3) + "\n";
                debug += string.Format("properties as float: {0:0.##} | {1:0.##} | {2:0.##} | {3:0.##}\n",
                    topBlock.getPropertyAsFloat(0),
                    topBlock.getPropertyAsFloat(1),
                    topBlock.getPropertyAsFloat(2),
                    topBlock.getPropertyAsFloat(3));
            }
        }
        debug += "mousePosition = " + (Vector2Int)mousePosition + "\n" +
            "chunkPosition = " + (Vector2Int)chunkPosition + "\n" +
            "blockPosition = " + (Vector2Int)blockPosition + "\n";
        debugText.text = debug;
    }

    public void Load(bool newMap)
    {
        mapLoaded = true;
        gridMap.Load(SceneData.Instance.MapName, newMap);
        DrawVisibleChunk(observerChunkPosition);
        DrawMinimap(observerChunkPosition);
    }

    public void Save()
    {
        mapLoaded = false;
        gridMap.Save();
        chunkCache.Clear();
    }

    private void CreateOnMissChunk(Vector3Int chunkPos, ref Chunk chunk)
    {
        MapGenerator.CreateChunkOnProperty(chunkPos, ref chunk);
    }

    private void OnGridmapSetValue(Vector3Int pos)
    {
        RenderedChunk rendered;
        Vector3Int chunkPosition, BlockPosition;
        Chunk.SplitPosition(pos, out chunkPosition, out BlockPosition);
        if (!chunkCache.TryGetValue(chunkPosition, out rendered))
        {
            rendered = chunkCache.Fetch();
            rendered.ChunkPos = chunkPosition;
            rendered.gameObject.SetActive(true);
            chunkCache.Submit(chunkPosition);
        }
        rendered.DrawAndUpdateNeighbor(BlockPosition, gridMap[pos]);
    }

    private void DrawChunkLater(Vector3Int chunkPos)
    {
        loadQueue.Enqueue(chunkPos);
    }

    public bool DrawChunk(Vector3Int chunkPos)
    {
        Chunk chunk = gridMap.GetChunk(chunkPos);
        if (chunk == null || chunk.isEmpty)
        {
            return false;
        }
        RenderedChunk rendered;
        if (!chunkCache.TryGetValue(chunkPos, out rendered))
        {
            rendered = chunkCache.Fetch();
            rendered.ChunkPos = chunkPos;
            rendered.gameObject.SetActive(true);
            chunkCache.Submit(chunkPos);
        }
        for (int y = 0; y < Chunk.chunkSize; y++) 
            for (int x = 0; x < Chunk.chunkSize; x++)
            {
                rendered.DrawAndUpdateNeighborAcrossBlock(x, y, chunk[x, y]);
            }
        return true;
    }

    public void setObserverPosition(Vector3Int position)
    {
        Vector3Int newObserverChunkPosition = new Vector3Int(position.x, position.y, 0);
        if (newObserverChunkPosition == observerChunkPosition)
        {
            return;
        }
        DrawVisibleChunk(newObserverChunkPosition);
        DrawMinimap(newObserverChunkPosition);

        observerChunkPosition = newObserverChunkPosition;
    }

    private void DrawVisibleChunk(Vector3Int observerPosition)
    {
        for (int col = -1; col <= 1; col++)
        {
            for (int row = -1; row <= 1; row++)
            {
                for (int layer = layerBottom; layer <= layerTop; layer++)
                {
                    Vector3Int c = new Vector3Int(row + observerPosition.x, col + observerPosition.y, layer);
                    if (!chunkCache.ContainsKey(c))
                    {
                        DrawChunkLater(c);
                    }
                    else
                    {
                        chunkCache.Refresh(c);
                    }
                }
            }
        }
    }

    public void DrawMinimap(Vector3Int chunkPos)
    {
        int chunkViewDistance = 4;
        for (int y = -chunkViewDistance; y <= chunkViewDistance; y++)
            for (int x = -chunkViewDistance; x <= chunkViewDistance; x++)
            {
                Chunk chunk = gridMap.GetChunk(chunkPos + new Vector3Int(x, y, 0));
                if (chunk != null)
                {
                    for (int yy = 0; yy < Chunk.chunkSize; yy++)
                        for (int xx = 0; xx < Chunk.chunkSize; xx++)
                        {
                            minimap.sprite.texture.SetPixel(
                                (chunkViewDistance + x) * Chunk.chunkSize + xx,
                                (chunkViewDistance + y) * Chunk.chunkSize + yy,
                                colors[chunk[xx, yy].Id]);
                        }
                }
            }
        minimap.sprite.texture.Apply();
    }

    public Block GetBlock(Vector3Int pos)
    {
        return gridMap[pos];
    }

    public void SetBlock(Vector3Int pos, Block block)
    {
        gridMap[pos] = block;
    }

    public Block GetLoadedBlock(Vector3Int pos)
    {
        return gridMap.GetLoadedBlock(pos);
    }

    public Chunk GetLoadedChunk(Vector3Int chunkPos)
    {
        return gridMap.GetLoadedChunk(chunkPos);
    }

    public RenderedChunk GetLoadedRenderedChunk(Vector3Int chunkPos)
    {
        RenderedChunk rendered;
        chunkCache.TryGetValue(chunkPos, out rendered);
        return rendered;
    }
}
