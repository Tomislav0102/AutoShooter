using Sirenix.OdinInspector;
using UnityEngine;

public class EventBus : MonoBehaviour
{
    public static System.Action OnLevelLoaded;
    public static System.Action<Brain, GenChange> OnBrainAddRemove;
    public static System.Action OnUltimateActivated;
    
}
