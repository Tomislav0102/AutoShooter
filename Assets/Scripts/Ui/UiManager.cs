using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class UiManager : MonoBehaviour
{
    [SerializeField] Image backgroundPanel;
    [SerializeField] UiPanel[] allPanels;
    MyDuo<PanelType, UiPanel> _pairPanels;
    public Transform PanelByType(PanelType panel) => _pairPanels.GetValueByKey(panel).transform;
    public Transform barContainer;
    public Transform pointersContainer;
    [SerializeField] Transform floatingContainer;
    
    public RectTransform offScreenPointerPrefab;
    public RectTransform statusPrefab;
    public RectTransform healthBarPrefab;
    public RectTransform numDisplayPrefab;
    [SerializeField] FloatingText floatingTextPrefab;
    [SerializeField] TextMeshProUGUI playerInfo;
    Transform _playerInfoTr;
    public List<Stats> _stats = new List<Stats>();
    
    public TomoJoystick.Joystick joystick;
    public UltimateUi ultimateUi;

    
    [Title("Debug")]
    [SerializeField] Toggle psToggle;
    [SerializeField] Toggle floatToggle;

    
    void Awake()
    {
        _pairPanels = new MyDuo<PanelType, UiPanel>();
        for (int i = 0; i < allPanels.Length; i++)
        {
            _pairPanels.Add(allPanels[i].myPanel, allPanels[i]);
        }

        playerInfo.text = "";
        _playerInfoTr = playerInfo.transform;
    }
    void Start()
    {
        OpenPanel(PanelType.Game);
        psToggle.isOn = Ga.me.gameData.showParticles;
        floatToggle.isOn = Ga.me.gameData.showFloatingInfo;

    }

    void LateUpdate()
    {
        if (_stats.Count == 0) return;
        Vector3 targetPos = Ga.me.team.playersBrain.myTransform.position + 2.5f * Vector3.up;
        _playerInfoTr.position = Ga.me.camRig.cam.WorldToScreenPoint(targetPos);

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

    public void FloatText(Vector3 spawnPoint, MyDuo<Element, float> damage, float offsetY = 2f)
    {
        if (!Ga.me.gameData.showFloatingInfo) return;
        Ga.me.pool.FloatingText().StartMe(spawnPoint, damage, offsetY);
    }
    public void FloatText(Vector3 spawnPoint, string st, Color col, float offsetY = 2f)
    {
        if (!Ga.me.gameData.showFloatingInfo) return;
        Ga.me.pool.FloatingText().StartMe(spawnPoint, st, col, offsetY);
    }

    public void ToggleParticles()
    {
        Ga.me.gameData.showParticles = psToggle.isOn;
    }
    public void ToggleFloatUI()
    {
        Ga.me.gameData.showFloatingInfo = floatToggle.isOn;
    }

    public void StatsInject(GenChange change, Stats stat)
    {
        switch (change)
        {
            case GenChange.Add:
                _stats.Add(stat);
                break;
            case GenChange.Remove:
                if (_stats.Contains(stat)) _stats.Remove(stat);
                break;
        }

        playerInfo.text = string.Empty;
        foreach (Stats item in _stats)
        {
            playerInfo.text += item + "\n";
        }
    }
}
