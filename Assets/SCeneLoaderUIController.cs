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

    private string selectedMap;

    public string SelectedMap { get => selectedMap; set => selectedMap = value; }

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
        DirectoryInfo di = new DirectoryInfo(Application.streamingAssetsPath + "/map/");
        DirectoryInfo[] dis = di.GetDirectories();
        for (int i = 0; i < dis.Length; i++)
        {
            string mapName = dis[i].Name;
            GameObject go = Instantiate(MapListItem, MapList);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, i * rt.rect.height);
            MapListItem mli = go.GetComponent<MapListItem>();
            mli.MapName = mapName;
            mli.UIController = this;
        }
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

    public void LoadWindowLoad()
    {
        SceneData.Instance.MapName = selectedMap;
        loader.StartGame();
    }

    public void LoadWindowBack()
    {
        LoadWindow.gameObject.SetActive(false);
        MainWindow.gameObject.SetActive(true);
    }

    public void NewWindowConfirm()
    {
        SceneData.Instance.MapName = mapName.text;
        SceneData.Instance.NewMap = true;
        SceneData.Instance.RandmonSeed = int.Parse(randomSeed.text) ;
        loader.StartGame();
    }

    public void NewWindowBack()
    {
        LoadWindow.gameObject.SetActive(false);
        NewWindow.gameObject.SetActive(true);
    }
}
