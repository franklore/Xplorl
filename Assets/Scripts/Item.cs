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
}