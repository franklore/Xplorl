using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackUIController : MonoBehaviour
{
    public GameObject player;

    public GameObject[] slots;

    public Image selectBox;

    private Pack pack;

    private void Start()
    {
        pack = player.GetComponent<Pack>();
        pack.registerUpdateEvent(UpdatePackUI);
        UpdatePackUI();
    }

    private void Update()
    {
        
    }

    public void UpdatePackUI()
    {
        selectBox.rectTransform.anchoredPosition = new Vector2(-5 + pack.SelectedItemIndex * 70, -5);
        for (int i = 0; i < pack.packCapacity; i++)
        {
            Text text = slots[i].transform.Find("Text").GetComponent<Text>();
            Image image = slots[i].transform.Find("Image").GetComponent<Image>();
            if (!pack[i].IsEmpty())
            {
                image.sprite = ItemObjectFactory.Instance.GetItemObject(pack[i].id).sprite;
                image.color = Color.white;
                text.text = "" + pack[i].count;
            }
            else
            {
                image.color = Color.clear;
                text.text = "";
            }
        }
    }
}
