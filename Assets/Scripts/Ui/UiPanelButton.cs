using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class UiPanelButton : MonoBehaviour
{
    [SerializeField] Button myButton;
    [SerializeField] TextMeshProUGUI myText;
    [SerializeField] bool changeName = true;
    [ShowIf(nameof(changeName))] [SerializeField] string infoPrefix;
    [SerializeField] PanelType panelType;

    void Start()
    {
        myButton.onClick.AddListener(() => { Ga.me.uiManager.OpenPanel(panelType); });
        if (changeName && myText is not null) myText.text = infoPrefix + " " + panelType.ToString();
    }
}
