using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void SwitchToGameScene()
    {
        SceneManager.LoadScene("Game");
        if (MusicManager.instance != null)
        {
            MusicManager.instance.LowerVolumeForGame();
        }
    }

    public void SwitchToMenuScene()
    {
        SceneManager.LoadScene("Menu");
        if (MusicManager.instance != null)
        {
            MusicManager.instance.IncreaseVolumeForMenu();
        }
    }

}
