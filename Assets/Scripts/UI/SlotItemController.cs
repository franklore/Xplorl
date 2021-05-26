using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotItemController : MonoBehaviour
{
    public Text text;
    public Image image;



    public void UpdateSlotItem(Item item)
    {
        if (!item.IsEmpty())
        {
            image.sprite = ItemObjectFactory.Instance.GetItemObject(item.id).sprite;
            image.color = Color.white;
            text.text = item.count >= 2 ? "" + item.count : "";
        }
        else
        {
            image.color = Color.clear;
            text.text = "";
        }
    }
}
