using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class NumberSelector : MonoBehaviour
{
    public Slider slider;

    public Text text;

    public Button confirmButton;

    public Button cancelButton;

    private UnityAction<int> confirmAction;

    private void Awake()
    {
        confirmButton.onClick.AddListener(Confirm);
    }

    public void Init(int maxCount)
    {
        slider.maxValue = maxCount;
        slider.minValue = 0;
        slider.value = Mathf.CeilToInt(maxCount / 2.0f);
        setText();
    }

    public void setText()
    {
        text.text = slider.value.ToString();
    }

    public void registerConfirmEvent(UnityAction<int> confirmAction)
    {
        this.confirmAction = confirmAction;
    }

    public void Confirm()
    {
        confirmAction.Invoke((int)slider.value);
    }

    public void Cancel()
    {
        confirmAction.Invoke(0);
    }
}
