using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pack : MonoBehaviour
{
    [SerializeField]
    private Item[] items;

    public int packCapacity;

    private int selectedItemIndex;

    public delegate void OnUpdatePack();

    private event OnUpdatePack updatePack;

    public void registerUpdateEvent(OnUpdatePack updatePack)
    {
        this.updatePack += updatePack;
    }

    public Item this[int index] 
    {
        get => items[index];
    }

    public int SelectedItemIndex {
        get => selectedItemIndex;
        set
        {
            selectedItemIndex = value;
            updatePack();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        items = new Item[packCapacity];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // return remain count;
    public int AddItem(int id, int count)
    {
        ItemObject io = ItemObjectFactory.Instance.GetItemObject(id);
        for (int i = 0; i < packCapacity; i++)
        {
            if (items[i].IsEmpty())
            {
                items[i].id = id;
            }
            if (items[i].id == id)
            {
                int space = io.maxCount - items[i].count;
                if (space >= count)
                {
                    items[i].count += count;
                    updatePack.Invoke();
                    return 0;
                }
                else
                {
                    items[i].count = io.maxCount;
                    count -= space;
                }
            }
        }
        updatePack.Invoke();
        return count;
    }

    public bool ConsumeItem(int id, int count)
    {
        int total = 0;
        for (int i = 0; i < packCapacity; i++)
        {
            if (items[i].id == id)
            {
                total += items[i].count;
            }
        }
        if (total < count)
        {
            return false;
        }

        total = count;
        for (int i = 0; i < packCapacity; i++)
        {
            if (items[i].id == id)
            {
                if (items[i].count > total)
                {
                    items[i].count -= total;
                    updatePack.Invoke();
                    return true;
                }
                else
                {
                    total -= items[i].count;
                    items[i].count = 0;
                }
            }
        }
        updatePack.Invoke();
        return true;
    }
}
