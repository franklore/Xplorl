using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemObjectFactory", menuName = "Items/ItemObjectFactory")]
public class ItemObjectFactory : ScriptableObject
{
    [SerializeField]
    private ItemObject[] itemObjects;

    private ItemObject[] indexedItemObjects;

    private static ItemObjectFactory instance;

    public GameObject DroppedItemTemplate;

    private void OnEnable()
    {
        indexedItemObjects = new ItemObject[itemObjects.Length];

        foreach (ItemObject io in itemObjects)
        {
            Debug.Log("load item id:" + io.id);
            indexedItemObjects[io.id] = io;
        }
    }

    public ItemObject GetItemObject(int id)
    {
        return indexedItemObjects[id];
    }

    public static ItemObjectFactory Instance { 
        get
        {
            if (instance == null)
            {
                instance = Resources.Load("Items/ItemObjectFactory") as ItemObjectFactory;
            }
            return instance;
        }
    }

    public GameObject CreateDroppedItem(Item item, Vector3 position)
    {
        GameObject go = Instantiate(DroppedItemTemplate, position, Quaternion.identity);
        DroppedItem di = go.GetComponent<DroppedItem>();
        di.Item = item;
        return go;
    }
}
