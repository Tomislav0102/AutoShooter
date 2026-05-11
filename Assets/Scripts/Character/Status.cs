using System;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour, IInit
{
    public enum Effect
    {
        Slowed, //attack and move speed
        Rooted, //move speed is 0, attack speed unaffected
        Stunned, //completely passive, enemy does nothing
        Confused, //attacks random character, changes target often, does not respond to aggro 
        Blinded, //like confused, but only close target
        Charmed, //behaves like summon
        Fumbling, //every attack misses
        Dripping_wet, //+elFire, -eIce, -elEle
        Dehydrated_dry, //-elFire, +elEle
        Freezing_cold,//+elFire, -elForce, +elPoison
        Sweltering_hot,//-elFire, +elIce, -elPoison
    }

    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            Refresh();
            IsInitialized = true;
        }
    }
    Brain _br;
    public bool IsInitialized { get; set; }
    List<Group> _statuses = new List<Group>();


    void Update()
    {
        int count = _statuses.Count;
        for (int i = 0; i < count; i++)
        {
            _statuses[i].duration -= Time.deltaTime;
            if (_statuses[i].duration <= 0) Change(GenChange.Remove, _statuses[i]);
        }
    }

    void Change(GenChange addRemove, Group status)
    {
        switch (addRemove)
        {
            case GenChange.Add:
                _statuses.Add(status);
                break;
            case GenChange.Remove:
                if (_statuses.Contains(status)) _statuses.Remove(status);
                break;
        }
        Refresh();
    }
    void Refresh()
    {
        foreach (Group item in _statuses)
        {
            switch (item.effect)
            {
                
            }
        }
    }
    
    class Group
    {
        public Effect effect;
        public float duration;
    }

}
