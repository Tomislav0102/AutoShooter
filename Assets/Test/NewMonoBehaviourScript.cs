using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] Guid _g;

    [Button]
    void Assign()
    {
        _g = Guid.NewGuid();
    }
    [Button]
    void Show()
    {
        print(_g);
    }
}
