using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class SCeneLoaderUIController : MonoBehaviour
{
    public RectTransform MapList;

    public GameObject MapListItem;

    public RectTransform MainWindow;

    public RectTransform LoadWindow;

    public RectTransform NewWindow;

    public InputField mapName;

    public InputField randomSeed;

    public SceneLoader loader;

    private string loadMapName;

    public string LoadMapName
    {
        get
        {
            return loadMapName;
        }
        set
        {
            loadMapName = value;
            textLoadMapName.text = loadMapName;
            MapDetailWindow.gameObject.SetActive(value != "");
        }
    }

    public RectTransform MapDetailWindow;

    public Text textLoadMapName;

    private string selectedMap;

    public string SelectedMap { get => selectedMap; set => selectedMap = value; }

    private void Start()
    {
        MainWindow.gameObject.SetActive(true);
        NewWindow.gameObject.SetActive(false);
        LoadWindow.gameObject.SetActive(false);
        MapDetailWindow.gameObject.SetActive(false);
    }

    public void MainWindowStart()
    {
        MainWindow.gameObject.SetActive(false);
        LoadWindow.gameObject.SetActive(true);
        LoadMapList();
    }

    private void LoadMapList()
    {
        for (int i = 0; i < MapList.childCount; i++)
        {
           Destroy(MapList.GetChild(i).gameObject);
        }
        DirectoryInfo di = new DirectoryInfo(SceneData.Instance.settings.mapRootDirectory);
        DirectoryInfo[] dis = di.GetDirectories();
        for (int i = 0; i < dis.Length; i++)
        {
            string mapName = dis[i].Name;
            GameObject go = Instantiate(MapListItem, MapList);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, -i * rt.rect.height);
            MapListItem mli = go.GetComponent<MapListItem>();
            mli.MapName = mapName;
            mli.SceneLoaderUIController = this;
        }
        MapList.sizeDelta = new Vector2(0, dis.Length * 100);
    }

    public void MainWindowQuit()
    {
        Application.Quit();
    }

    public void LoadWindowNew()
    {
        LoadWindow.gameObject.SetActive(false);
        NewWindow.gameObject.SetActive(true);
    }

    public void LoadWindowBack()
    {
        LoadWindow.gameObject.SetActive(false);
        MainWindow.gameObject.SetActive(true);
    }

    public void LoadWindowLoad()
    {
        SceneData.Instance.mapName = loadMapName;
        SceneData.Instance.isNewMap = false;
        loader.StartGame();
    }

    public void LoadWindowDelete()
    {
        string path = Path.Combine(SceneData.Instance.settings.mapRootDirectory, loadMapName);
        Directory.Delete(path, true);
        LoadMapName = "";
        LoadMapList();
    }

    public void NewWindowConfirm()
    {
        SceneData.Instance.mapName = mapName.text;
        SceneData.Instance.isNewMap = true;
        SceneData.Instance.mapData.randomSeed = RandomGenerator.StringToSeed(randomSeed.text);
        loader.StartGame();
    }

    public void NewWindowBack()
    {
        NewWindow.gameObject.SetActive(false);
        LoadWindow.gameObject.SetActive(true);
    }
}
