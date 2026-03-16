using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TestApp
{
    [DefaultExecutionOrder(-10)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public TomoJoystick.Joystick joystick;
        public Transform plTransform;

        void Awake()
        {
            Instance = this;
        }

        public void ButtonQuit()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
