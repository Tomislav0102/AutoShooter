using System;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public enum Phase
    {
        UnInitialized,
        Initialized,
        Running,
        Ended
    }
    [SerializeField] Transform parFloating;
    [SerializeField] FloatingText prefabFloatingText;
    FloatingText[] _floatingTexts;
    int _counterFloatingTexts;
    public FloatingText FloatingText()
    {
        FloatingText ft = _floatingTexts[_counterFloatingTexts];
        _counterFloatingTexts = (1 + _counterFloatingTexts) % _floatingTexts.Length;
        return ft;
    }


    void Awake()
    {
        _floatingTexts = new FloatingText[100];
        for (int i = 0; i < _floatingTexts.Length; i++)
        {
            _floatingTexts[i] = Instantiate(prefabFloatingText, parFloating);
        }
    }
}
