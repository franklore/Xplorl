using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PackUIController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private GameObject player;

    private GameObject[] slots;

    public GameObject SlotPrefab;

    private float slotWidth;

    public GameObject SelectedBoxPrefab;

    private float selectBoxBorderWidth;

    public GameObject NumberSelectorPrefab;

    public GameObject bottomSlots;

    public GameObject otherSlots;

    public RectTransform selectBoxRect;

    private GameObject numberSelector;

    private RectTransform rt;

    public bool Expanded { get => otherSlots.activeSelf; }

    private Pack pack;

    public int packCapacity;

    public int slotRowCount = 4;

    public static PackUIController Instance;

    public GameObject Player
    {
        get => player; set
        {
            player = value;
            pack = player.GetComponent<Pack>();
            pack.registerUpdateEvent(UpdatePackUI);
            UpdatePackUI();
        }
    }

    private void Awake()
    {
        Instance = this;
        rt = GetComponent<RectTransform>();
        otherSlots.SetActive(false);

        slotWidth = SlotPrefab.GetComponent<RectTransform>().sizeDelta.x;
        selectBoxBorderWidth = (selectBoxRect.sizeDelta.x - slotWidth) / 2;
        numberSelector = Instantiate(NumberSelectorPrefab, transform);
        numberSelector.SetActive(false);

        slots = new GameObject[slotRowCount * 10];
        for (int row = 0; row < slotRowCount; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                GameObject slot = Instantiate(SlotPrefab);
                slot.name = "slot" + row + "" + col;
                RectTransform rect = slot.GetComponent<RectTransform>();
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0);
                rect.pivot = new Vector2(0, 0);
                if (row == 0)
                {
                    rect.SetParent(bottomSlots.transform);
                    rect.anchoredPosition = new Vector2((col - 5) * slotWidth, row * slotWidth);
                }
                else
                {
                    rect.SetParent(otherSlots.transform);
                    rect.anchoredPosition = new Vector2((col - 5) * slotWidth, (row - 1) * slotWidth);
                }

                rect.localScale = Vector3.one;
                slots[row * 10 + col] = slot;
            }
        }
    }

    private void Update()
    {
        PickedSlotItemFollowsPointer();
    }

    public void Expand()
    {
        otherSlots.SetActive(true);
    }

    public void Collapse()
    {
        otherSlots.SetActive(false);
    }

    public void UpdatePackUI()
    {
        selectBoxRect.anchoredPosition = new Vector2(
            (pack.SelectedItemIndex - 5) * slotWidth - selectBoxBorderWidth, -selectBoxBorderWidth);
        for (int i = 0; i < pack.packCapacity; i++)
        {
            slots[i].GetComponent<SlotItemController>().UpdateSlotItem(pack[i]);
        }
    }

    private int clickedSlotIndex;

    private GameObject pickedSlotItem;

    private RectTransform pickedSlotItemRect;

    private int pickedCount;

    private Vector2 offset;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (pickedSlotItem != null)
        {
            PutSlotItem();
        }
        if (numberSelector.activeSelf)
        {
            numberSelector.SetActive(false);
        }
        else if (Expanded)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Vector2 pointerPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.enterEventCamera, out pointerPos);
                clickedSlotIndex = ScreenPointToSlotIndex(pointerPos);
                offset = pointerPos - new Vector2(Mathf.Floor(pointerPos.x / slotWidth) + 0.5f, Mathf.Floor(pointerPos.y / slotWidth)) * slotWidth;

                pickedSlotItem = Instantiate(SlotPrefab, transform);
                pickedSlotItemRect = pickedSlotItem.GetComponent<RectTransform>();
                pickedSlotItem.GetComponent<SlotItemController>().UpdateSlotItem(pack[clickedSlotIndex]);
                slots[clickedSlotIndex].gameObject.SetActive(false);
                pickedCount = pack[clickedSlotIndex].count;
            }
            else if (!numberSelector.activeSelf)
            {
                Vector2 pointerPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.enterEventCamera, out pointerPos);
                clickedSlotIndex = ScreenPointToSlotIndex(pointerPos);
                offset = pointerPos - new Vector2(Mathf.Floor(pointerPos.x / slotWidth) + 0.5f, Mathf.Floor(pointerPos.y / slotWidth)) * slotWidth;
                if (pack[clickedSlotIndex].count > 0)
                {

                    numberSelector.SetActive(true);
                    RectTransform nsrt = numberSelector.GetComponent<RectTransform>();
                    nsrt.anchoredPosition = pointerPos;
                    NumberSelector ns = numberSelector.GetComponent<NumberSelector>();
                    ns.Init(pack[clickedSlotIndex].count);
                    ns.registerConfirmEvent(NumberSelectorConfirmed);
                }
            }
        }
    }

    private void NumberSelectorConfirmed(int value)
    {
        if (value != 0)
        {
            pickedCount = value;
            pickedSlotItem = Instantiate(SlotPrefab, transform);
            pickedSlotItemRect = pickedSlotItem.GetComponent<RectTransform>();
            Item pickedItem = pack[clickedSlotIndex];
            pickedItem.count = value;
            Item remainItem = pack[clickedSlotIndex];
            remainItem.count -= value;
            pickedSlotItem.GetComponent<SlotItemController>().UpdateSlotItem(pickedItem);
            slots[clickedSlotIndex].GetComponent<SlotItemController>().UpdateSlotItem(remainItem);
        }
        numberSelector.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PutSlotItem();
    }

    private void PutSlotItem()
    {
        if (pickedSlotItem != null)
        {
            Vector2 pointerPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, null, out pointerPos);
            if (ScreenPointInsidePack(pointerPos))
            {
                slots[clickedSlotIndex].gameObject.SetActive(true);
                int destSlotIndex = ScreenPointToSlotIndex(pointerPos);
                pack.Combine(clickedSlotIndex, destSlotIndex, pickedCount);
                Destroy(pickedSlotItem);
            }
            else
            {
                slots[clickedSlotIndex].gameObject.SetActive(true);
                UpdatePackUI();
                Destroy(pickedSlotItem);
            }
        }
    }

    private void PickedSlotItemFollowsPointer()
    {
        if (pickedSlotItem != null)
        {
            Vector2 pointerPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, null, out pointerPos);
            pickedSlotItemRect.anchoredPosition = pointerPos - offset;
        }
    }

    private int ScreenPointToSlotIndex(Vector2 pos)
    {
        return Mathf.FloorToInt(pos.y / slotWidth) * 10 + Mathf.FloorToInt(pos.x / slotWidth) + 5;
    }

    private bool ScreenPointInsidePack(Vector2 pos)
    {
        if (Expanded)
        {
            return pos.x >= -slotWidth * 5 && pos.y <= slotWidth * 5 && pos.y >= 0 && pos.y <= slotWidth * 4;
        }
        else
        {
            return pos.x >= -slotWidth * 5 && pos.y <= slotWidth * 5 && pos.y >= 0 && pos.y <= slotWidth;
        }
    }
}
