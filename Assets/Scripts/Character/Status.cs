using System;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour, IIniBrain
{

    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            _groups = new List<Group>();
        }
    }
    Brain _br;
    
    List<Group> _groups;


    void Update()
    {
        int count = _groups.Count;
        for (int i = 0; i < count; i++)
        {
            Group g =  _groups[i];
            g.duration -= Time.deltaTime;
            if (g.duration > 0) continue;
            Change(GenChange.Remove, g);
        }
    }
    void Change(GenChange addRemove, Group group)
    {
        switch (addRemove)
        {
            case GenChange.Add:
                _groups.Add(group);
                break;
            case GenChange.Remove:
                if (_groups.Contains(group)) _groups.Remove(group);
                break;
        }
        Refresh();
    }


    void Refresh()
    {
        
    }


    class Group
    {
        public StatusEffect statusEffect;
        public float duration;
        public int intensity;

        public Group(StatusEffect statusEffect, float duration = float.PositiveInfinity, int intensity = 0)
        {
            this.statusEffect = statusEffect;
            this.duration = duration;
            this.intensity = intensity;
        }
    }
}
