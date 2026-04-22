using Sirenix.OdinInspector;
using UnityEngine;

public class EventBus : MonoBehaviour
{
    public static System.Action OnLevelLoaded;
    public static System.Action<Transform> OnCharDeath;
    public static System.Action OnPlayerDeath;
    public static System.Action OnUltimateActivated;
    
    protected virtual void OnEnable()
    {
        OnCharDeath += CallEv_OnCharDeath;
        OnPlayerDeath += CallEv_OnPlayerDeath;
        OnLevelLoaded += CallEv_OnLevelLoaded;
    }

    protected virtual void OnDisable()
    {
        OnCharDeath -= CallEv_OnCharDeath;
        OnPlayerDeath -= CallEv_OnPlayerDeath;
        OnLevelLoaded -= CallEv_OnLevelLoaded;
    }

    protected virtual void CallEv_OnCharDeath(Transform tr)
    {
       
    }

    protected virtual void CallEv_OnPlayerDeath()
    {
        
    }
    protected virtual void CallEv_OnLevelLoaded() { }
}
