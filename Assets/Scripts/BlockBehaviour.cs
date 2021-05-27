using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{

    public RenderedChunk renderedChunk;

    private Vector3Int localPosition;

    private GameObject hitFx;

    private AudioClip hitSound;

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

    private SpriteRenderer sr;

    private void Awake()
    {
        co = gameObject.AddComponent<BoxCollider2D>();
        co.offset = new Vector2(0, 0);
        co.size = new Vector2(1, 1);
        dr = gameObject.AddComponent<DamageReceiver>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void UpdateComponents(Block block)
    {
        BlockObject bo = BlockFactory.Instance.GetBlockObject(block);
        BlockObject.SpriteData spriteData = bo.GetSpriteData(Position);
        sr.sprite = spriteData.sprite;
        sr.transform.position = new Vector3(Position.x, Position.y, 0) 
            + new Vector3(0.5f, 0.5f, 0)
            + bo.randomOffset * (Vector3)RandomGenerator.RandomVec2(Position, BlockMap.Instance.mapData.randomSeed + 2);
        sr.transform.rotation = Quaternion.Euler(0, 0, -90 * spriteData.rotation);
        if (bo.isEntityBlock)
        {
            sr.sortingOrder = 0;
            sr.sortingLayerName = "Entity";
        }
        else
        {
            sr.sortingOrder = renderedChunk.ChunkPos.z;
            sr.sortingLayerName = "Terrain";
        }
        co.enabled = bo.isCollider;
        dr.enabled = bo.receiveDamage;
        hitFx = bo.hitFx;
        hitSound = bo.hitSound;
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
        Vector3 randomOffset = Random.insideUnitCircle;
        Vector3 hitPoint = transform.position + randomOffset;
        Instantiate(hitFx, hitPoint, Quaternion.identity);
        AudioSource.PlayClipAtPoint(hitSound, hitPoint);
    }
}
