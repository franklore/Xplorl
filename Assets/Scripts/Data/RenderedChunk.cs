using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RenderedChunk : MonoBehaviour
{
    private Vector3Int chunkPos;
    //public SpriteRenderer spriteRenderer;
    private GameObject[,] renderedBlocks;

    public Vector3Int ChunkPos
    {
        get { return chunkPos; }
        set
        {
            chunkPos = value;
            transform.position = new Vector3Int(chunkPos.x * Chunk.chunkSize, chunkPos.y * Chunk.chunkSize, 0);
        }
    }

    private void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        renderedBlocks = new GameObject[Chunk.chunkSize, Chunk.chunkSize];
        for (int y = 0; y < Chunk.chunkSize; y++)
            for (int x = 0; x < Chunk.chunkSize; x++)
            {
                GameObject go = new GameObject("Block_" + x + "_" + y);
                SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
                renderer.spriteSortPoint = SpriteSortPoint.Pivot;
                go.transform.parent = transform;
                BlockBehaviour bh = go.AddComponent<BlockBehaviour>();
                bh.renderedChunk = this;
                bh.LocalPosition = new Vector3Int(x, y, 0);
                renderedBlocks[x, y] = go;
            }
    }

    public void Clear()
    {
        gameObject.SetActive(false);
        foreach (GameObject renderedBlock in renderedBlocks)
        {
            BlockBehaviour bh = renderedBlock.GetComponent<BlockBehaviour>();
            bh.Clear();
        }
    }

    public void Draw(int x, int y, Block block)
    {
        //Debug.Log("draw (" + x + "," + y + "): " + block);
        BlockBehaviour bb = renderedBlocks[x, y].GetComponent<BlockBehaviour>();
        bb.UpdateComponents(block);
    }

    public void Draw(Vector3Int pos, Block block)
    {
        Draw(pos.x, pos.y, block);
    }

    public void DrawAndUpdateNeighbor(Vector3Int pos, Block block)
    {
        DrawAndUpdateNeighbor(pos.x, pos.y, block);
    }

    public void DrawAndUpdateNeighbor(int x, int y, Block block)
    {
        Vector3Int blockPos = Chunk.CombinePosition(ChunkPos, x, y);
        Draw(x, y, block);
        Redraw(blockPos + new Vector3Int(-1, -1, 0));
        Redraw(blockPos + new Vector3Int(-1, 0, 0));
        Redraw(blockPos + new Vector3Int(-1, 1, 0));
        Redraw(blockPos + new Vector3Int(0, -1, 0));
        Redraw(blockPos + new Vector3Int(0, 1, 0));
        Redraw(blockPos + new Vector3Int(1, -1, 0));
        Redraw(blockPos + new Vector3Int(1, 0, 0));
        Redraw(blockPos + new Vector3Int(1, 1, 0));
    }

    public void DrawAndUpdateNeighborAcrossBlock(Vector3Int pos, Block block) {
        DrawAndUpdateNeighborAcrossBlock(pos.x, pos.y, block);
    }

    public void DrawAndUpdateNeighborAcrossBlock(int x, int y, Block block)
    {
        Draw(x, y, block);
        int z = chunkPos.z;
        Vector3Int blockPos = Chunk.CombinePosition(chunkPos, new Vector3Int(x, y, z));
        if (x == 0)
        {
            if (y == 0)
            {
                RedrawIfId(block.Id, blockPos + new Vector3Int(-1, -1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(-1, 0, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(-1, 1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(0, -1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(1, -1, 0));
            }
            else if (y == Chunk.chunkSize - 1)
            {
                RedrawIfId(block.Id, blockPos + new Vector3Int(-1, -1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(-1, 0, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(-1, 1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(0, 1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(1, 1, 0));
            }
            else
            {
                RedrawIfId(block.Id, blockPos + new Vector3Int(-1, -1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(-1, 0, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(-1, 1, 0));
            }
        }
        else if (x == Chunk.chunkSize - 1)
        {
            if (y == 0)
            {
                RedrawIfId(block.Id, blockPos + new Vector3Int(1, -1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(1, 0, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(1, 1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(-1, -1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(0, -1, 0));
            }
            else if (y == Chunk.chunkSize - 1)
            {
                RedrawIfId(block.Id, blockPos + new Vector3Int(1, -1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(1, 0, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(1, 1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(-1, 1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(0, 1, 0));
            }
            else
            {
                RedrawIfId(block.Id, blockPos + new Vector3Int(1, -1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(1, 0, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(1, 1, 0));
            }
        }
        else
        {
            if (y == 0)
            {
                RedrawIfId(block.Id, blockPos + new Vector3Int(-1, -1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(0, -1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(1, -1, 0));
            }
            else if (y == Chunk.chunkSize - 1)
            {
                RedrawIfId(block.Id, blockPos + new Vector3Int(-1, 1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(0, 1, 0));
                RedrawIfId(block.Id, blockPos + new Vector3Int(1, 1, 0));
            }
        }
    }


    private void Redraw(Vector3Int pos)
    {
        Vector3Int chunkPosition, blockPosition;
        Chunk.SplitPosition(pos, out chunkPosition, out blockPosition);
        RenderedChunk renderedChunk = BlockMap.Instance.GetLoadedRenderedChunk(chunkPosition);
        Block block = BlockMap.Instance.GetLoadedBlock(pos);
        if (renderedChunk != null)
        {
            renderedChunk.Draw(blockPosition, block);
        }
    }

    private void RedrawIfId(int id, Vector3Int pos)
    {
        Block block = BlockMap.Instance.GetLoadedBlock(pos);
        if (block != null && block.Id == id)
        {
            Redraw(pos);
        }
    }
}
