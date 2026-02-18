using System;
using UnityEngine;

public class EventBus : MonoBehaviour
{
    public static System.Action<Transform> OnAllyDeath;
    public static System.Action OnPLayerDeath;
    protected GameManager gm;


    protected virtual void Awake()
    {
        gm = GameManager.Instance;
    }

    protected virtual void OnEnable()
    {
        OnAllyDeath += CallEv_OnAllyDeath;
        OnPLayerDeath += CallEv_OnPLayerDeath;
    }

    protected virtual void OnDisable()
    {
        OnAllyDeath -= CallEv_OnAllyDeath;
        OnPLayerDeath -= CallEv_OnPLayerDeath;
    }

    protected virtual void CallEv_OnAllyDeath(Transform tr)
    {
       
    }

    protected virtual void CallEv_OnPLayerDeath()
    {
        
    }
}
