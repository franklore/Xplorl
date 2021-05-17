using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMain : MonoBehaviour
{
    public void Quit()
    {
        BlockMap.Instance.Save();
        SceneManager.LoadScene("Start");
    }
}
