using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] SoGameData gameData;
    [SerializeField] TextMeshProUGUI enemiesCountText;


    void Start()
    {
        if (!PlayerPrefs.HasKey(gameData.prefsTestEnemyCount))
        {
            PlayerPrefs.SetInt(gameData.prefsTestEnemyCount, 1);
        }
        enemiesCountText.text = PlayerPrefs.GetInt(gameData.prefsTestEnemyCount).ToString();
    }

    public void BtnPlay()
    {
        SceneManager.LoadScene(gameData.sceneGame);
    }

    public void BtnChangeCount(bool increase)
    {
        int count = PlayerPrefs.GetInt(gameData.prefsTestEnemyCount);
        if (increase) count++;
        else count--;
        if (count < 0) count = 0;
        PlayerPrefs.SetInt(gameData.prefsTestEnemyCount,  count);
        enemiesCountText.text = PlayerPrefs.GetInt(gameData.prefsTestEnemyCount).ToString();
    }
}
