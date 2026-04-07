using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;


public class EnergyManager : MonoBehaviour
{
    public SoGameData gameData;
    public TextMeshProUGUI totalCounterText, singleCounterText, energyText;
    int Energy
    {
        get => _energy;
        set
        {
            _energy = value;
            energyText.text = $"{value}/{energyMax}";
        }
    }
    [ShowInInspector, ReadOnly] int _energy;
    public int energyMax;
    public int secondsToLoad;
    DateTime _startTime;
    DateTime _finalTime;
    TimeSpan Difference() => _finalTime - DateTime.UtcNow;

    void Awake()
    {
        InitializeMe();
    }

    void InitializeMe()
    {
        if (!PlayerPrefs.HasKey(gameData.prefsEnergyStartTime))
        {
            PlayerPrefs.SetString(gameData.prefsEnergyStartTime, DateTime.UtcNow.ToBinary().ToString());
        }
        long startTime = Convert.ToInt64(PlayerPrefs.GetString(gameData.prefsEnergyStartTime));
        _startTime = DateTime.FromBinary(startTime);
        
        if (!PlayerPrefs.HasKey(gameData.prefsEnergyFinishTime))
        {
            PlayerPrefs.SetString(gameData.prefsEnergyFinishTime, DateTime.UtcNow.ToBinary().ToString());
        }
        long finalTime = Convert.ToInt64(PlayerPrefs.GetString(gameData.prefsEnergyFinishTime));
        _finalTime = DateTime.FromBinary(finalTime);
    }

    void Update()
    {
        double totalSeconds = Difference().TotalSeconds;
        
        if (totalSeconds <= 0)
        {
            totalCounterText.text = "Energy fully recovered!";
            Energy = energyMax;
            return;
        }
        Energy = energyMax - Mathf.CeilToInt((float)totalSeconds / secondsToLoad);
        
        double hours = Difference().Hours;
        double minutes = Difference().Minutes;
        double seconds = Difference().Seconds;
        totalCounterText.text = $"Total time remaining - {hours:00}:{minutes:00}:{seconds:00}";

        TimeSpan startDifference = DateTime.UtcNow - _startTime;
        double fromStartSeconds = startDifference.TotalSeconds;
        if (fromStartSeconds <= 0) return;
        double effStartSeconds = secondsToLoad - fromStartSeconds % secondsToLoad;
        singleCounterText.text = $"{effStartSeconds:00}";
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) InitializeMe();
    }

    public void Button_UseEnergy()
    {
        if (Energy == 0)
        {
            print("Energy is 0");
            return;
        }
        if (Difference().TotalSeconds <= 0)
        {
            _startTime = DateTime.UtcNow;
            PlayerPrefs.SetString(gameData.prefsEnergyStartTime, DateTime.UtcNow.ToBinary().ToString());
            _finalTime = DateTime.UtcNow;
        }
        
        _finalTime = _finalTime.AddSeconds(secondsToLoad);
        PlayerPrefs.SetString(gameData.prefsEnergyFinishTime, _finalTime.ToBinary().ToString());
    }

    public void Button_Reset()
    {
        PlayerPrefs.SetString(gameData.prefsEnergyStartTime, DateTime.UtcNow.ToBinary().ToString());
        PlayerPrefs.SetString(gameData.prefsEnergyFinishTime, DateTime.UtcNow.ToBinary().ToString());
    }

}

