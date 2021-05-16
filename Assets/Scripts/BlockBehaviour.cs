using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{

    public RenderedChunk renderedChunk;

    private Vector3Int localPosition;

    public Vector3Int Position
    {
        get
        {
            return Chunk.CombinePosition(renderedChunk.ChunkPos, LocalPosition);
        }
    }

    public Block block
    {
        get => BlockMap.Instance.GetBlock(Position);
        set => BlockMap.Instance.SetBlock(Position, value);
    }
    public Vector3Int LocalPosition
    {
        get { return localPosition; }
        set
        {
            localPosition = value;
        }
    }

    private BoxCollider2D co = null;

    private DamageReceiver dr = null;

    private SpriteRenderer renderer;

    private void Awake()
    {
        co = gameObject.AddComponent<BoxCollider2D>();
        co.offset = new Vector2(0, 0);
        co.size = new Vector2(1, 1);
        dr = gameObject.AddComponent<DamageReceiver>();
        renderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void UpdateComponents(Block block)
    {
        BlockObject bo = BlockFactory.Instance.GetBlockObject(block);
        BlockObject.SpriteData spriteData = bo.GetSpriteData(Position);
        renderer.sprite = spriteData.sprite;
        renderer.transform.position = new Vector3(Position.x, Position.y, 0) 
            + new Vector3(0.5f, 0.5f, 0)
            + bo.randomOffset * (Vector3)RandomGenerator.RandomVec2(Position);
        renderer.transform.rotation = Quaternion.Euler(0, 0, -90 * spriteData.rotation);
        if (bo.isEntityBlock)
        {
            renderer.sortingOrder = 0;
            renderer.sortingLayerName = "Entity";
        }      
        else
        {
            renderer.sortingOrder = renderedChunk.ChunkPos.z;
            renderer.sortingLayerName = "Terrain";
        }

        co.enabled = bo.isCollider;
        dr.enabled = bo.receiveDamage;
        //if (bo.isCollider && co == null)
        //{
        //    co = gameObject.AddComponent<BoxCollider2D>();
        //    co.offset = new Vector2(0, 0);
        //    co.size = new Vector2(1, 1);
        //}
        //if (!bo.isCollider && co != null)
        //{
        //    Destroy(co);
        //}

        //if (bo.receiveDamage && dr == null)
        //{
        //    dr = gameObject.AddComponent<DamageReceiver>();
        //}
        //if (!bo.receiveDamage && dr != null)
        //{
        //    Destroy(dr);
        //}
    }

    public void Clear()
    {
        co.enabled = false;
        dr.enabled = false;
    }

    private void OnReceiveDamage(Damage damage)
    {
        BlockObject bo = BlockFactory.Instance.GetBlockObject(block);
        bo.DamageBlock(this, damage);
    }
}
