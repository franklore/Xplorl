using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    private static SceneData instance;

    public static SceneData Instance { get => instance; }

    public bool isNewMap;

    public string mapName;

    public MapData mapData;

    public GameSettings settings;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        settings.mapRootDirectory = Application.persistentDataPath + "/maps/";
    }
}
