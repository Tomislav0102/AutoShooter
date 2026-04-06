using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class Test : MonoBehaviour
{
    public int energy, energyMax;
    public int secondsToAdd;
    public MyTime begin;
    public MyTime end;
    
    [Button]
    void TestMethod()
    {
        DateTime startCountdown = new DateTime(2000, 1, 1, begin.hour, begin.minute, begin.second);
        startCountdown = startCountdown.AddSeconds(secondsToAdd);
        end = new MyTime(startCountdown.Hour, startCountdown.Minute, startCountdown.Second);
    }


}

[System.Serializable]
public struct MyTime
{
    public int hour, minute, second;

    public MyTime(int hour, int minute, int second)
    {
        this.hour = hour;
        this.minute = minute;
        this.second = second;
    }
}


// layerMask = (1 << layer); //make layer a layermask
// layerMask |= (1 << layer); //add layer to layermask
// layerMask &= ~(1 << layer); //remove layer from layermask
