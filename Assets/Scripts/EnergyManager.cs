using UnityEngine;
using Sirenix.OdinInspector;
using System;
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
    DateTime _finalTime;
    TimeSpan Difference() => _finalTime - DateTime.Now;

    void Awake()
    {
        if (!PlayerPrefs.HasKey(gameData.prefsEnergyFinishTime))
        {
            PlayerPrefs.SetString(gameData.prefsEnergyFinishTime, DateTime.Now.ToString());
        }
        _finalTime = DateTime.Parse(PlayerPrefs.GetString(gameData.prefsEnergyFinishTime));
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
        totalCounterText.text = $"{hours:00}:{minutes:00}:{seconds:00}";
        
        TimeSpan singleDiff = TimeSpan.FromSeconds(seconds);
        singleCounterText.text = $"{hours:00}:{minutes:00}:{seconds:00}";
    }
    
    public void Button_UseEnergy()
    {
        if (Energy == 0)
        {
            print("Energy is 0");
            return;
        }
        if (Difference().TotalSeconds <= 0) _finalTime = DateTime.Now;
        
        _finalTime = _finalTime.AddSeconds(secondsToLoad);
        PlayerPrefs.SetString(gameData.prefsEnergyFinishTime, _finalTime.ToString());
    }

    public void Button_Reset()
    {
        PlayerPrefs.SetString(gameData.prefsEnergyFinishTime, DateTime.Now.ToString());
    }

}



// public class EnergyManager : MonoBehaviour
// {
//     public SoGameData gameData;
//     public TextMeshProUGUI counterText, energyText;
//     int Energy
//     {
//         get => _energy;
//         set
//         {
//             _energy = value;
//             PlayerPrefs.SetInt(gameData.prefsEnergyCurrent, value);
//             energyText.text = $"{value}/{energyMax}";
//         }
//     }
//     [ShowInInspector, ReadOnly] int _energy;
//     public int energyMax;
//     public int secondsToLoad;
//     DateTime _finalTime;
//     TimeSpan Difference() => _finalTime - DateTime.Now;
//
//     void Awake()
//     {
//         if (!PlayerPrefs.HasKey(gameData.prefsEnergyFinishTime))
//         {
//             PlayerPrefs.SetString(gameData.prefsEnergyFinishTime, DateTime.Now.ToString());
//         }
//         _finalTime = DateTime.Parse(PlayerPrefs.GetString(gameData.prefsEnergyFinishTime));
//
//         if (!PlayerPrefs.HasKey(gameData.prefsEnergyCurrent))
//         {
//             Energy = energyMax;
//         }
//         else
//         {
//             int energyCurrent = PlayerPrefs.GetInt(gameData.prefsEnergyCurrent);
//             if (energyCurrent == energyMax)
//             {
//                 Energy = energyMax;
//                 return;
//             }
//             
//             DateTime finalTimeWithAddedEnergy = _finalTime.AddSeconds(secondsToLoad * (energyMax - energyCurrent));
//             TimeSpan difference = finalTimeWithAddedEnergy - DateTime.Now;
//             Energy = Mathf.Min(Mathf.FloorToInt((float)difference.TotalSeconds / secondsToLoad), energyMax);
//         }
//         
//     }
//
//     void Update()
//     {
//         if (Energy == energyMax) return;
//         
//         double totalSeconds = Difference().TotalSeconds;
//         
//         if (totalSeconds <= 0)
//         {
//             Energy++;
//             if (Energy == energyMax)
//             {
//                 counterText.text = "Energy fully recovered!";
//                 return;
//             }
//             AddFinalTime();
//         }
//         
//         double hours = Difference().Hours;
//         double minutes = Difference().Minutes;
//         double seconds = Difference().Seconds;
//         counterText.text = $"{hours:00}:{minutes:00}:{seconds:00}";
//     }
//     
//     public void Button_UseEnergy()
//     {
//         if (Energy == 0)
//         {
//             print("Energy is 0");
//             return;
//         }
//         Energy--;
//         if (Energy == energyMax - 1) AddFinalTime();
//     }
//
//     void AddFinalTime()
//     {
//         if (Difference().TotalSeconds <= 0) _finalTime = DateTime.Now;
//         
//         _finalTime = _finalTime.AddSeconds(secondsToLoad);
//         PlayerPrefs.SetString(gameData.prefsEnergyFinishTime, _finalTime.ToString());
//     }
//
//     public void Button_Reset()
//     {
//         PlayerPrefs.SetString(gameData.prefsEnergyFinishTime, DateTime.Now.ToString());
//         Energy = energyMax;
//     }
//
// }


// public class EnergyManager : MonoBehaviour
// {
//     public SoGameData gameData;
//     public TextMeshProUGUI counterText, energyText;
//     public int energy;
//     public int energyMax;
//     public int secondsToLoad;
//     DateTime _finalTime;
//     TimeSpan Difference() => _finalTime - DateTime.Now;
//
//     void Awake()
//     {
//         if (!PlayerPrefs.HasKey(gameData.prefsEnergyFinishTime))
//         {
//             PlayerPrefs.SetString(gameData.prefsEnergyFinishTime, DateTime.Now.ToString());
//         }
//         _finalTime = DateTime.Parse(PlayerPrefs.GetString(gameData.prefsEnergyFinishTime));
//     }
//
//     void Update()
//     {
//         double totalSeconds = Difference().TotalSeconds;
//         
//         energyText.text = $"{energy:0.000}/{energyMax}";
//         
//         if (totalSeconds <= 0)
//         {
//             counterText.text = "Energy fully recovered!";
//             return;
//         }
//         energy = Mathf.FloorToInt((float)totalSeconds * energyMax / secondsToLoad);
//         
//         double hours = Difference().Hours;
//         double minutes = Difference().Minutes;
//         double seconds = Difference().Seconds;
//         counterText.text = $"{hours:00}:{minutes:00}:{seconds:00}";
//     }
//     
//     public void Button_UseEnergy()
//     {
//         if (Difference().TotalSeconds >= secondsToLoad)
//         {
//             print("no energy");
//             return;
//         }
//         if (Difference().TotalSeconds <= 0) _finalTime = DateTime.Now;
//         
//         _finalTime = _finalTime.AddSeconds(secondsToLoad);
//         PlayerPrefs.SetString(gameData.prefsEnergyFinishTime, _finalTime.ToString());
//     }
//
//     public void Button_Reset()
//     {
//         PlayerPrefs.SetString(gameData.prefsEnergyFinishTime, DateTime.Now.ToString());
//     }
//
// }
