using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class ItemObject : ScriptableObject
{
    public int id;

    public int maxCount;

    public Sprite sprite;

    public virtual void SelectItem(ItemOperationInfo info)
    {

    }

    public virtual void DeselectItem(ItemOperationInfo info)
    {

    }

    public virtual void UseItemStart(ItemOperationInfo info)
    {

    }

    public virtual void UseItemEnd(ItemOperationInfo info)
    {

    }

    public virtual Item CreateItem(int count)
    {
        return new Item(id, count, -1);
    }

    public virtual string getDescription(Item item)
    {
        return "id: " + id + "\n" +
            item.count + "/" + maxCount;

    }
}

public struct ItemOperationInfo
{
    public GameObject invoker;

    public GameObject entity;

    public Vector3 operationPosition;

    public Item item;
}