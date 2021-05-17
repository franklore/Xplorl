using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapListItem : MonoBehaviour
{
    private string mapName;

    private SCeneLoaderUIController ui;

    public SCeneLoaderUIController UIController
    {
        get => ui; set
        {       
            ui = value;
            Button button = GetComponent<Button>();
            button.onClick.AddListener(SetMapName);
        }
    }

    public string MapName
    {
        get => mapName; set
        {
            mapName = value;
            Text text = transform.GetChild(0).GetComponent<Text>();
            text.text = mapName;
        }
    }

    private void SetMapName()
    {
        ui.SelectedMap = mapName;
    }
}
