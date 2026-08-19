using System;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour, IIniBrain
{
    /// <summary>
    /// DOT have intensity, and they stack
    /// Others don't have intensity and only durations stack
    /// </summary>
    public enum Effect
    {
        Invulnerable,
        Slowed, //attack and move speed
        Rooted, //move speed is 0, attack speed unaffected
        Stunned, //completely passive, enemy does nothing
        Confused, //attacks random character, changes target often, does not respond to aggro 
        Blinded, //like confused, but only close target
        Charmed, //behaves like summon
        Fumbling, //every attack misses
        // Dripping_wet, //+elFire, -eIce, -elEle
        // Dehydrated_dry, //-elFire, +elEle
        // Freezing_cold,//+elFire, -elForce, +elPoison
        // Sweltering_hot,//-elFire, +elIce, -elPoison
        DOT_Bleeding,
        DOT_Burning,
        DOT_Freezing,
        DOT_Jolted,
        DOT_Poisoned
    }

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
        public Effect effect;
        public float duration;
        public int intensity;

        public Group(Effect effect, float duration = float.PositiveInfinity, int intensity = 0)
        {
            this.effect = effect;
            this.duration = duration;
            this.intensity = intensity;
        }
    }
}
