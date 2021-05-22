using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pack : MonoBehaviour
{
    [SerializeField]
    private Item[] items;

    public int packCapacity;

    private int selectedItemIndex;

    public Item SelectedItem
    {
        get => this[selectedItemIndex];
    }

    public delegate void OnUpdatePack();

    private event OnUpdatePack updatePack;

    public void registerUpdateEvent(OnUpdatePack updatePack)
    {
        this.updatePack += updatePack;
    }

    public Item this[int index]
    {
        get => items[index];
        set
        {
            items[index] = value;
            updatePack.Invoke();
        }
    }

    public int SelectedItemIndex
    {
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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public int AddItem(Item item)
    {
        return AddItem(item.id, item.count);
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

    public bool ContainsItem(Item item)
    {
        return ContainsItem(item.id, item.count);
    }

    public bool ContainsItem(int id, int count)
    {
        int total = 0;
        for (int i = 0; i < packCapacity; i++)
        {
            if (items[i].id == id)
            {
                total += items[i].count;
                if (total >= count)
                {
                    return true;
                }
            }
        }
        return false;

    }

    public bool ConsumeItem(Item item)
    {
        return ConsumeItem(item.id, item.count);
    }

    public bool ConsumeItem(int id, int count)
    {
        if (!ContainsItem(id, count))
        {
            return false;
        }
        int total = count;
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

    public bool ConsumeItemAtIndex(int index, int count)
    {
        if (items[index].count >= count)
        {
            items[index].count -= count;
            updatePack.Invoke();
            return true;
        }
        return false;
    }

    public void Clear()
    {
        items = new Item[packCapacity];
        updatePack.Invoke();
    }

    public void InitPack(Item[] items)
    {
        this.items = new Item[packCapacity];
        for (int i = 0; i < items.Length; i++)
        {
            this.items[i] = items[i];
        }
        this.packCapacity = this.items.Length;
        updatePack.Invoke();
    }

    public bool Combine(int sourceIndex, int destIndex)
    {
        return Combine(sourceIndex, destIndex, this[sourceIndex].count);
    }

    public bool Combine(int sourceIndex, int destIndex, int count)
    {
        if (items[destIndex].IsEmpty())
        {
            items[destIndex].id = items[sourceIndex].id;
        }
        if (count > items[sourceIndex].count)

        {
            return false;
        }
        if (items[destIndex].id != items[sourceIndex].id)
        {
            if (items[sourceIndex].count == count)
            {
                Item temp = items[destIndex];
                items[destIndex] = items[sourceIndex];
                items[sourceIndex] = temp;
                updatePack.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }

        ItemObject io = ItemObjectFactory.Instance.GetItemObject(items[sourceIndex].id);
        if (this[destIndex].count + count > io.maxCount)
        {
            int combined = io.maxCount - items[destIndex].count;
            items[destIndex].count += combined;
            items[sourceIndex].count -= combined;
        }
        else
        {
            items[destIndex].count += count;
            items[sourceIndex].count -= count;
        }
        updatePack.Invoke();
        return true;
    }

    public void ApplyRecipe(Recipe recipe)
    {
        for (int i = 0; i < recipe.Ingredients.Length; i++)
        {
            if (!ContainsItem(recipe.Ingredients[i]))
            {
                return;
            }
        }
        for (int i = 0; i < recipe.Ingredients.Length; i++)
        {
            ConsumeItem(recipe.Ingredients[i]);
        }
        for (int i = 0; i < recipe.Products.Length; i++)
        {
            AddItem(recipe.Products[i]);
        }
    }
}
