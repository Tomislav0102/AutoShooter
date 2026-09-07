using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] SoGameData gameData;
    [SerializeField] TextMeshProUGUI enemiesCountText;
    [SerializeField] Toggle psToggle;
    [SerializeField] TMP_Dropdown playerDropdown;

    void Start()
    {
        if (!PlayerPrefs.HasKey(gameData.prefsTestEnemyCount))
        {
            PlayerPrefs.SetInt(gameData.prefsTestEnemyCount, 1);
        }
        enemiesCountText.text = PlayerPrefs.GetInt(gameData.prefsTestEnemyCount).ToString();

        if (!PlayerPrefs.HasKey(gameData.prefsTestChosenPlayer))
        {
            PlayerPrefs.SetInt(gameData.prefsTestChosenPlayer, 0);
        }
        playerDropdown.value = PlayerPrefs.GetInt(gameData.prefsTestChosenPlayer);
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

    public void DropDownChosePlayer()
    {
        PlayerPrefs.SetInt(gameData.prefsTestChosenPlayer, playerDropdown.value);
    }
    public void ToggleParticles()
    {
        gameData.showParticles = psToggle.isOn;
    }
}
