using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockFactory
{
    public int spriteSize = 16;

    private BlockObject[] indexedBlockObjects;

    private static BlockFactory instance = new BlockFactory();

    public static BlockFactory Instance
    {
        get
        {
            return instance;
        }
    }

    private BlockFactory()
    {
        object[] objects = Resources.LoadAll("Blocks");

        indexedBlockObjects = new BlockObject[objects.Length];

        foreach (BlockObject bo in objects)
        {
            Debug.Log("load block id:" + bo.id);
            indexedBlockObjects[bo.id] = bo;
        }
    }

    public BlockObject GetBlockObject(Block block)
    {
        return indexedBlockObjects[block == null ? 0 : block.Id];
    }

    public BlockObject GetBlockObject(int id)
    {
        return indexedBlockObjects[id];
    }

    public void CreateBlock(int id, int state, ref Block block)
    {
        indexedBlockObjects[id].CreateBlock(state, ref block);
    }
}
