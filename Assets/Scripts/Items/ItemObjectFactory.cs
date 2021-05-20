using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemObjectFactory", menuName = "Items/ItemObjectFactory")]
public class ItemObjectFactory : ScriptableObject
{
    private ItemObject[] indexedItemObjects;

    private static ItemObjectFactory instance;

    public static ItemObjectFactory Instance
    {
        get
        {
            return instance;
        }
    }

    public GameObject DroppedItemTemplate;

    private void OnEnable()
    {
        object[] objects = Resources.LoadAll("Items");

        indexedItemObjects = new ItemObject[objects.Length];

        foreach (ItemObject io in objects)
        {
            Debug.Log("load item id:" + io.id);
            indexedItemObjects[io.id] = io;
        }
        instance = this;
    }


    public ItemObject GetItemObject(int id)
    {
        return indexedItemObjects[id];
    }

    public GameObject CreateDroppedItem(Item item, Vector3 position)
    {
        GameObject go = Instantiate(DroppedItemTemplate, position, Quaternion.identity);
        DroppedItem di = go.GetComponent<DroppedItem>();
        di.Item = item;
        return go;
    }
}
