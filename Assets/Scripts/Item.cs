using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Item
{
    public int id;

    public int count;

    public int entityId;


    public Item(int id, int count, int entityId)
    {
        this.id = id;
        this.count = count;
        this.entityId = entityId;
    }

    public bool IsEmpty()
    {
        return count == 0;
    }

    public static bool operator== (Item i1, Item i2)
    {
        return i1.id == i2.id && i1.count == i2.count && i1.entityId == i2.entityId;
    }

    public static bool operator!= (Item i1, Item i2)
    {
        return !(i1 == i2);
    }
}