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

    public delegate void Action();

    private event Action updatePack;

    public delegate void ActionInt(int i);

    private event ActionInt updateSelectedItem;

    public void registerUpdateEvent(Action updatePack)
    {
        this.updatePack += updatePack;
    }

    public void registerUpdateSelectedItem(ActionInt updateSelectedItem)
    {
        this.updateSelectedItem += updateSelectedItem;
    }

    public Item this[int index]
    {
        get => items[index];
        set
        {
            items[index] = value;
            updatePack?.Invoke();
        }
    }

    public int SelectedItemIndex
    {
        get => selectedItemIndex;
        set
        {
            if (selectedItemIndex != value)
            {
                int old = selectedItemIndex;
                selectedItemIndex = value;
                updatePack?.Invoke();
                updateSelectedItem?.Invoke(old);
            }
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
        Item oldSelect = SelectedItem;
        ItemObject io = ItemObjectFactory.Instance.GetItemObject(item.id);

        int count = item.count;
        for (int i = 0; i < packCapacity; i++)
        {
            if (items[i].IsEmpty())
            {
                if (item.count <= io.maxCount)
                {
                    items[i] = item;
                    count = 0;
                    break;
                }
                else
                {
                    items[i].id = item.id;
                }   
            }
            if (items[i].id == item.id)
            {
                int space = io.maxCount - items[i].count;
                if (space >= count)
                {
                    items[i].count += count;
                    break;
                }
                else
                {
                    items[i].count = io.maxCount;
                    count -= space;
                }
            }
        }
        updatePack?.Invoke();
        if (SelectedItem != oldSelect)
        {
            updateSelectedItem?.Invoke(selectedItemIndex);
        }
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
        Item oldSelect = SelectedItem;
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
                    updatePack?.Invoke();
                    return true;
                }
                else
                {
                    total -= items[i].count;
                    items[i].count = 0;
                }
            }
        }
        updatePack?.Invoke();
        if (oldSelect != SelectedItem)
        {
            updateSelectedItem?.Invoke(selectedItemIndex);
        }
        return true;
    }

    public bool ConsumeItemAtIndex(int index, int count)
    {
        if (items[index].count >= count)
        {
            Item oldSelect = SelectedItem;
            items[index].count -= count;
            updatePack?.Invoke();
            if (oldSelect != SelectedItem)
            {
                updateSelectedItem?.Invoke(selectedItemIndex);
            }
            return true;
        }
        return false;
    }

    public void Clear()
    {
        Item oldSelect = SelectedItem;
        items = new Item[packCapacity];
        updatePack?.Invoke();
        if (oldSelect != SelectedItem)
        {
            updateSelectedItem?.Invoke(selectedItemIndex);
        }
    }

    public void InitPack(Item[] items)
    {
        Item oldSelect = SelectedItem;
        this.items = new Item[packCapacity];
        for (int i = 0; i < items.Length; i++)
        {
            this.items[i] = items[i];
        }
        this.packCapacity = this.items.Length;
        updatePack.Invoke();
        if (oldSelect != SelectedItem)
        {
            updateSelectedItem?.Invoke(selectedItemIndex);
        }
    }

    public bool Combine(int sourceIndex, int destIndex)
    {
        return Combine(sourceIndex, destIndex, this[sourceIndex].count);
    }

    public bool Combine(int sourceIndex, int destIndex, int count)
    {
        Item oldSelect = SelectedItem;
        if (items[destIndex].IsEmpty())
        {
            items[destIndex].id = items[sourceIndex].id;
        }
        if (count > items[sourceIndex].count)
        {
            return false;
        }

        ItemObject iosrc = ItemObjectFactory.Instance.GetItemObject(items[sourceIndex].id);
        ItemObject iodst = ItemObjectFactory.Instance.GetItemObject(items[destIndex].id);

        if (items[destIndex].id != items[sourceIndex].id || iosrc.maxCount == 1 || iodst.maxCount == 1)
        {
            if (items[sourceIndex].count == count)
            {
                Item temp = items[destIndex];
                items[destIndex] = items[sourceIndex];
                items[sourceIndex] = temp;
                updatePack?.Invoke();
                if (oldSelect != SelectedItem)
                {
                    updateSelectedItem?.Invoke(selectedItemIndex);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        if (this[destIndex].count + count > iosrc.maxCount)
        {
            int combined = iosrc.maxCount - items[destIndex].count;
            items[destIndex].count += combined;
            items[sourceIndex].count -= combined;
        }
        else
        {
            items[destIndex].count += count;
            items[sourceIndex].count -= count;
        }
        updatePack?.Invoke();
        if (oldSelect != SelectedItem)
        {
            updateSelectedItem?.Invoke(selectedItemIndex);
        }
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
