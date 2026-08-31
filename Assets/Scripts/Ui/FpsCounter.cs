using UnityEngine;
using TMPro;

public class FpsCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fpsText;
    float _timer;
    int Fps
    {
        get => _fps;
        set
        {
            _fps = value;
            fpsText.text = $"FPS: {_fps}";
        }
    }
    int _fps;
    int _frameCounter;

    void Update()
    {
        _timer += Time.deltaTime;
        _frameCounter++;
        if (_timer >= 1f)
        {
            Fps = _frameCounter;
            _frameCounter = 0;
            _timer = 0;
        }

    }

}
