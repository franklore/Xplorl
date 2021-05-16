using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "BlockFactory", menuName = "Block/BlockFactory")]
public class BlockFactory : ScriptableObject
{
    [SerializeField]
    private BlockObject[] blockObjects;

    public int spriteSize = 16;

    private BlockObject[] indexedBlockObjects;

    private static BlockFactory instance;

    public GameObject Shadow;

    public static BlockFactory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load("Blocks/BlockFactory") as BlockFactory;
            }
            return instance;
        }
    }
    private void OnEnable()
    {
        indexedBlockObjects = new BlockObject[blockObjects.Length];

        foreach (BlockObject bo in blockObjects)
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
