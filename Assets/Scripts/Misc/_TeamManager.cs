using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class TeamManager
{
    public Transform playerTransform;
    HashSet<Brain> _good;
    HashSet<Brain> _bad;
    HashSet<Brain> _neutral;

    public TeamManager()
    {
        _good = new HashSet<Brain>();
        _bad = new HashSet<Brain>();
        _neutral = new HashSet<Brain>();
    }
    
    public void Death(Brain brain)
    {
        RemovalFromTeam(brain);
    }
    void RemovalFromTeam(Brain brain)
    {
        bool canRemove = false;
        if (_good.Contains(brain))
        {
            _good.Remove(brain);
            canRemove = true;
        }
        else if (_bad.Contains(brain))
        {
            _bad.Remove(brain);
            canRemove = true;
        }
        else if (_neutral.Contains(brain))
        {
            _neutral.Remove(brain);
            canRemove = true;
        }
        if (canRemove) Ga.OnBrainAddRemove?.Invoke(brain, GenChange.Remove);
    }

    public void JoinTeam(Faction newFaction, Brain brain)
    {
        RemovalFromTeam(brain);
        switch (newFaction)
        {
            case Faction.GoodGuys:
                _good.Add(brain);
                break;
            case Faction.BadGuys:
                _bad.Add(brain);
                break;
            case Faction.Neutral:
                _neutral.Add(brain);
                break;
        }
        Ga.OnBrainAddRemove?.Invoke(brain, GenChange.Add);
    }

    public int TeamMemberCount(Faction faction)
    {
        switch (faction)
        {
            case Faction.GoodGuys:
                return _good.Count;
            case Faction.BadGuys:
                return _bad.Count;
            case Faction.Neutral:
                return _neutral.Count;
            default:
                return 0;
        }
    }
    
    public HashSet<Transform> ValidTargets(Faction faction)
    {
        HashSet<Transform> temp =  new HashSet<Transform>();
        HashSet<Brain> brains =  validTargetBrain();
        
        foreach (Brain item in brains)
        {
            temp.Add(item.myTransform);
        }
        return temp;
        
        HashSet<Brain> validTargetBrain()
        {
            switch (faction)
            {
                case Faction.GoodGuys:
                    return  _bad;
                case Faction.BadGuys:
                    return  _good;
                default:
                    return null;
            }
        }
    }

}