using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapListItem : MonoBehaviour
{
    private string mapName;

    private SceneLoader loader;

    public SceneLoader SceneLoader
    {
        get => loader; set
        {
            loader = value;
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
        SceneData.Instance.mapName = mapName;
        SceneData.Instance.isNewMap = false;
        loader.StartGame();
    }
}
