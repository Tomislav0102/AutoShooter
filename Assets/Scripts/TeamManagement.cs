using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class TeamManagement : MonoBehaviour
{
    [ReadOnly] public Transform playerTransform;
    HashSet<Brain> _good = new HashSet<Brain>();
    HashSet<Brain> _bad = new HashSet<Brain>();
    HashSet<Brain> _neutral = new HashSet<Brain>();
    
    public HashSet<Transform> ValidTargets(Faction faction)
    {
        HashSet<Transform> temp =  new HashSet<Transform>();
        HashSet<Brain> brains =  ValidTargetBrain(faction);
        foreach (Brain item in brains)
        {
            temp.Add(item.myTransform);
        }
        
        return temp;
    }
    HashSet<Brain> ValidTargetBrain(Faction faction)
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

    void OnEnable()
    {
        EventBus.OnCharDeath += CallEv_OnCharDeath;
    }
    void OnDisable()
    {
        EventBus.OnCharDeath -= CallEv_OnCharDeath;
    }
    void CallEv_OnCharDeath(Brain brainDead)
    {
       ChangeTeam(brainDead.Faction, brainDead, GenChange.Remove);
    }


    
    public void ChangeTeam(Faction faction, Brain brain, GenChange change)
    {
        if (brain == null) return;
        
        HashSet<Brain> temp = new HashSet<Brain>();
        switch (faction)
        {
            case Faction.GoodGuys:
                temp =  _good;
                break;
            case Faction.BadGuys:
                temp =  _bad;
                break;
            case Faction.Neutral:
                temp =  _neutral;
                break;
        }
       
        if (_good.Contains(brain)) _good.Remove(brain);
        if (_bad.Contains(brain)) _bad.Remove(brain);
        if (_neutral.Contains(brain)) _neutral.Remove(brain);
        switch (change)
        {
            case GenChange.Add:
                temp.Add(brain);
                break;
            case GenChange.Remove:
                if (faction == Faction.GoodGuys)
                {
                    if (brain.myTransform == playerTransform)
                    {
                        EventBus.OnPlayerDeath?.Invoke();
                        if (Ga.me.debug) print("Player is dead");
                    }
                    else
                    {
                        if (Ga.me.debug) print("Summon is dead");
                    }
                }
                else
                {
                    if (Ga.me.debug) print("Enemy is dead");
                }
                break;
        }
    }
}