using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class ItemObject : ScriptableObject
{
    public int id;

    public int maxCount;

    public Sprite sprite;

    public int placedBlockId;
}
