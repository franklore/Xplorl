using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    private static SceneData instance;

    public static SceneData Instance { get => instance; }
    public string MapName { get => mapName; set => mapName = value; }
    public bool NewMap { get => newMap; set => newMap = value; }
    public int RandmonSeed { get => randmonSeed; set => randmonSeed = value; }

    [SerializeField]
    private string mapName;

    [SerializeField]
    private bool newMap;

    [SerializeField]
    private int randmonSeed;

    public void SetMapName(string mapName)
    {
        MapName = mapName;
    }

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
    }
}
