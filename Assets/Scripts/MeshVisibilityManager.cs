using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class MeshVisibilityManager : MonoBehaviour
{
    [SerializeField] bool visibleInEditor;
    List<Transform> _children;

    [Button]
    void SetInEditor()
    {
        _children = new List<Transform>();
        GetChildren(transform);
        SetVisibility(visibleInEditor);
    }
    [Button]
    void ToggleVisibility()
    {
        _children = new List<Transform>();
        GetChildren(transform);
        visibleInEditor = !visibleInEditor;
        SetVisibility(visibleInEditor);
    }

    void Awake()
    {
        _children = new List<Transform>();
        GetChildren(transform);
        SetVisibility(false);
    }

    void SetVisibility(bool visible)
    {
        foreach (Transform item in _children)
        {
            item.GetComponent<MeshRenderer>().enabled = visible;
        }

    }  
    void GetChildren(Transform par)
    {
        foreach (Transform item in par)
        {
            if (item.GetComponent<MeshRenderer>() != null) _children.Add(item);
            GetChildren(item);
        }
        
    }
}
