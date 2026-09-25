using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class UiManager : MonoBehaviour
{
    [SerializeField] Image backgroundPanel;
    [SerializeField] UiPanel[] allPanels;
    MyDuo<PanelType, UiPanel> _pairPanels;
    public Transform barContainer;
    public Transform pointersContainer;
    public Transform floatingContainer;
    
    public RectTransform offScreenPointerPrefab;
    public RectTransform statusPrefab;
    public RectTransform healthBarPrefab;
    public RectTransform numDisplayPrefab;
    public FloatingText floatingTextPrefab;
    
    public TomoJoystick.Joystick joystick;
    public UltimateUi ultimateUi;
    [HideInInspector] public LevelUpCard[] levelUpCards;

    [Title("Debug")]
    
    void Awake()
    {
        _pairPanels = new MyDuo<PanelType, UiPanel>();
        for (int i = 0; i < allPanels.Length; i++)
        {
            _pairPanels.Add(allPanels[i].myPanel, allPanels[i]);
        }
        levelUpCards = Utils.AllChildren<LevelUpCard>(_pairPanels.GetValueByKey(PanelType.LevelUp).transform.GetChild(0));
    }
    void Start()
    {
        OpenPanel(PanelType.Skills);
    }

    public void OpenPanel(PanelType panel)
    {
        Time.timeScale = panel == PanelType.Game ? 1 : 0;
        backgroundPanel.enabled = panel != PanelType.Game;
        backgroundPanel.transform.SetAsLastSibling();
        for (int i = 0; i < _pairPanels.Length(); i++)
        {
            _pairPanels.GetValue(i).IsOpen = false;
        }
        
        _pairPanels.GetValueByKey(panel).IsOpen = true;
        _pairPanels.GetValueByKey(panel).transform.SetAsLastSibling();
    }

    
    public void InjectSkills(SoSkill[] skills)
    {
        for (int i = 0; i < levelUpCards.Length; i++)
        {
            levelUpCards[i].InjectSkill(skills[i]);
        }
    }

}
