using UnityEngine;

public class UiPanel : MonoBehaviour
{
    public PanelType myPanel;
    [SerializeField] GameObject container;
    public bool IsOpen
    {
        get => _isOpen;
        set
        {
            _isOpen = value;
            container.SetActive(value);
        }
    }
    bool _isOpen;

}
