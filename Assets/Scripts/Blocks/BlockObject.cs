using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "BlockObject", menuName = "Block/BlockObject")]
public class BlockObject : ScriptableObject
{
    public int id;

    public int layer;

    public Sprite[] sprite;

    public enum SpriteType
    {
        None, 

        Single,

        State,

        Random,

        Animated,

        [Tooltip("Zero\n" +
            "One\n" +
            "Two\n" +
            "Three\n" +
            "Four"
            )]
        Pipe,

        [Tooltip("Filled\n" +
            "Three Sides\n" +
            "Two Sides and One Corner\n" +
            "Two Adjacent Sides\n" +
            "Opposite Sides\n" +
            "One Side and Two Corners\n" +
            "One Side and One Lower Corner\n" +
            "One Side and One Upper Corner\n" +
            "One Side\n" +
            "Four Corners\n" +
            "Three Corners\n"+
            "Two Adjacent Corners\n" +
            "Two Opposite Corners\n" +
            "One Corner\n" +
            "Empty")]
        Terrain,
    }

    public SpriteType spriteType;

    public float randomOffset;

    public bool isCollider;

    public bool receiveDamage;

    public bool isEntityBlock;

    public float maxHealth;

    public Item[] dropList;

    public virtual void CreateBlock(int state, ref Block block)
    {
        block.Id = id;
        block.State = state;
        block.Health = maxHealth;
    }

    public virtual void InteractBlock(Vector3Int position) 
    {

    }

    public virtual void DamageBlock(BlockBehaviour bh, Damage damage)
    {
        bh.block.Health -= damage.value;
        if (bh.block.Health <= 0)
        {
            bh.block.SetEmpty();
            bh.Clear();
            if (dropList != null)
            {
                foreach (Item item in dropList)
                {
                    GameObject dropped = ItemObjectFactory.Instance.CreateDroppedItem(item, bh.Position + new Vector3(0.5f, 0.5f, 0));
                }
            }
        }
    }

    public SpriteData GetSpriteData(Vector3Int pos)
    {
        SpriteData data = new SpriteData();
        if (spriteType == SpriteType.None)
        {
            return data;
        }
        if (spriteType == SpriteType.Single)
        {
            data.sprite = sprite[0];
            return data;
        }
        else if (spriteType == SpriteType.Random)
        {
            return GetRandomSpriteData(pos);
        }
        else if (spriteType == SpriteType.Pipe)
        {
            return GetPipeSpriteData(pos);
        }
        else if (spriteType == SpriteType.Terrain)
        {
            return GetTerrainSpriteData(pos);
        }
        else
        {
            data.sprite = sprite[0];
            return data;
        }
    }

    public struct SpriteData
    {
        public Sprite sprite;

        public int rotation;
    }

    private SpriteData GetRandomSpriteData(Vector3Int pos)
    {
        SpriteData spriteData = new SpriteData();
        spriteData.sprite = sprite[(int)(RandomGenerator.RandomValue(pos, 999) * sprite.Length)];
        return spriteData;
    } 


    private SpriteData GetPipeSpriteData(Vector3Int pos)
    {
        SpriteData data = new SpriteData();

        int mask = SameId(pos + new Vector3Int(0, 1, 0)) ? 1 : 0;
        mask += SameId(pos + new Vector3Int(1, 0, 0)) ? 2 : 0;
        mask += SameId(pos + new Vector3Int(0, -1, 0)) ? 4 : 0;
        mask += SameId(pos + new Vector3Int(-1, 0, 0)) ? 8 : 0;

        int index = GetPipeIndex((byte)mask);
        if (index >= 0 && index < sprite.Length && SameId(pos))
        {
            data.sprite = sprite[index];
            data.rotation = GetPipeRotation((byte)mask);
        }
        return data;
    } 
    private SpriteData GetTerrainSpriteData(Vector3Int pos)
    {
        SpriteData data = new SpriteData();
        int mask = SameId(pos + new Vector3Int(0, 1, 0)) ? 1 : 0;
        mask += SameId(pos + new Vector3Int(1, 1, 0)) ? 2 : 0;
        mask += SameId(pos + new Vector3Int(1, 0, 0)) ? 4 : 0;
        mask += SameId(pos + new Vector3Int(1, -1, 0)) ? 8 : 0;
        mask += SameId(pos + new Vector3Int(0, -1, 0)) ? 16 : 0;
        mask += SameId(pos + new Vector3Int(-1, -1, 0)) ? 32 : 0;
        mask += SameId(pos + new Vector3Int(-1, 0, 0)) ? 64 : 0;
        mask += SameId(pos + new Vector3Int(-1, 1, 0)) ? 128 : 0;

        byte original = (byte)mask;
        if ((original | 254) < 255) { mask = mask & 125; }
        if ((original | 251) < 255) { mask = mask & 245; }
        if ((original | 239) < 255) { mask = mask & 215; }
        if ((original | 191) < 255) { mask = mask & 95; }

        int index = GetTerrainIndex((byte)mask);
        if (index >= 0 && index < sprite.Length /*&& SameId(chunk, pos)*/)
        {
            data.sprite = sprite[index];
            data.rotation = mask == 255 ? 
                (int)(RandomGenerator.RandomValue(pos, 0) * 4) : 
                GetTerrainRotation((byte)mask);
        }
        return data;
    }

    private bool SameId(Vector3Int pos)
    {
        Block block = BlockMap.Instance.GetLoadedBlock(pos);
        return block != null && block.Id == id;
    }

    private int GetTerrainIndex(byte mask)
    {
        switch (mask)
        {
            case 0: return 0;
            case 1:
            case 4:
            case 16:
            case 64: return 1;
            case 5:
            case 20:
            case 80:
            case 65: return 2;
            case 7:
            case 28:
            case 112:
            case 193: return 3;
            case 17:
            case 68: return 4;
            case 21:
            case 84:
            case 81:
            case 69: return 5;
            case 23:
            case 92:
            case 113:
            case 197: return 6;
            case 29:
            case 116:
            case 209:
            case 71: return 7;
            case 31:
            case 124:
            case 241:
            case 199: return 8;
            case 85: return 9;
            case 87:
            case 93:
            case 117:
            case 213: return 10;
            case 95:
            case 125:
            case 245:
            case 215: return 11;
            case 119:
            case 221: return 12;
            case 127:
            case 253:
            case 247:
            case 223: return 13;
            case 255: return 14;
        }
        return -1;
    }

    private int GetTerrainRotation(byte mask)
    {
        switch (mask)
        {
            case 4:
            case 20:
            case 28:
            case 68:
            case 84:
            case 92:
            case 116:
            case 124:
            case 93:
            case 125:
            case 221:
            case 253:
                return 1;
                //return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -90f), Vector3.one);
            case 16:
            case 80:
            case 112:
            case 81:
            case 113:
            case 209:
            case 241:
            case 117:
            case 245:
            case 247:
                return 2;
                //return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -180f), Vector3.one);
            case 64:
            case 65:
            case 193:
            case 69:
            case 197:
            case 71:
            case 199:
            case 213:
            case 215:
            case 223:
                return 3;
                //return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -270f), Vector3.one);
        }
        return 0;
        //return Matrix4x4.identity;
    }

    private int GetPipeIndex(byte mask)
    {
        switch (mask)
        {
            case 0: return 0;
            case 3:
            case 6:
            case 9:
            case 12: return 2;
            case 1:
            case 2:
            case 4:
            case 8: return 1;
            case 5:
            case 10: return 3;
            case 7:
            case 11:
            case 13:
            case 14: return 4;
            case 15: return 5;
        }
        return -1;
    }

    private int GetPipeRotation(byte mask)
    {
        switch (mask)
        {
            case 2:
            case 6:
            case 10:
            case 14:
                return 1;
            case 4:
            case 12:
            case 13:
                return 2;
            case 8:
            case 9:
            case 11:
                return 3;
        }
        return 0;
    }
}
