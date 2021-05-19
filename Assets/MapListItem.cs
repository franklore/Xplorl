using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapListItem : MonoBehaviour
{
    private string mapName;

    private SCeneLoaderUIController controller;

    public SCeneLoaderUIController SceneLoaderUIController
    {
        get => controller; set
        {
            controller = value;
            Button button = GetComponent<Button>();
            button.onClick.AddListener(LoadGame);
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

    public void LoadGame()
    {
        controller.LoadMapName = mapName;
    }
}
