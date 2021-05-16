using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Item
{
    [SerializeField]
    public int id;

    [SerializeField]
    public int count;

    //Dictionary<string, object> properties;

    public Item(int id, int count)
    {
        this.id = id;
        this.count = count;
    }

    public static Item EmptyItem
    {
        get
        {
            return new Item(0, 0);
        }
    }

    public bool IsEmpty()
    {
        return count == 0;
    }
}