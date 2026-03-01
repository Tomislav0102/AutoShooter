using System;
using UnityEngine;

public class EventBus : MonoBehaviour
{
    public static System.Action<Transform> OnCharDeath;
    public static System.Action OnPlayerDeath;
    public static System.Action OnSpecialActivated;
    protected GameManager gm;


    protected virtual void Awake()
    {
        gm = GameManager.Instance;
    }

    protected virtual void OnEnable()
    {
        OnCharDeath += CallEv_OnCharDeath;
        OnPlayerDeath += CallEv_OnPlayerDeath;
    }

    protected virtual void OnDisable()
    {
        OnCharDeath -= CallEv_OnCharDeath;
        OnPlayerDeath -= CallEv_OnPlayerDeath;
    }

    protected virtual void CallEv_OnCharDeath(Transform tr)
    {
       
    }

    protected virtual void CallEv_OnPlayerDeath()
    {
        
    }
}
