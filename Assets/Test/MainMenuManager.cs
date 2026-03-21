using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TestApp
{

    public class MainMenuManager : MonoBehaviour
    {
        public TextMeshProUGUI ensText;
        int _counter;

        void Awake()
        {
            _counter = PlayerPrefs.GetInt("ens");
            ensText.text = _counter.ToString();
        }

        public void ArrowLeft()
        {
            _counter -= 25;
            PlayerPrefs.SetInt("ens", _counter);
            ensText.text = _counter.ToString();
        }
        public void ArrowRight()
        {
            _counter += 25;
            PlayerPrefs.SetInt("ens", _counter);
            ensText.text = _counter.ToString();
        }
        public void Play()
        {
            SceneManager.LoadScene("TestScene");
        }
    }
}
