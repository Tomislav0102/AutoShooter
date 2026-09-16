using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour, IIniBrain
{
    public enum Effect
    {
        Invulnerable,
        Rooted, //move speed is 0, attack speed unaffected
        Stunned, //completely passive, enemy does nothing
        Confused, //attacks random character, changes target often, does not respond to aggro 
        Blinded, //like confused, but only close target, movement is roam or idle
        Charmed, //behaves like summon
        Fumbling, //every attack misses
        Burning,
        Freezing,
        Jolted,
        Poisoned,
        Bleeding,
        //on weapons (mostly)
        InstantKill,
        Impact, //stunned enemies take 50% more damage
        Keen, //extra crit chance
        Serrated, //2X damage vs unarmored, 0.5X damage vs armored
        Explosive, //bonus knockback
    }

    RectTransform _status;
    GameObject[] _statusImageGos;
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            _groups = new List<EffectGroup>();
            _status = Instantiate(Ga.me.statusPrefab, Ga.me.parPointers);
            _statusImageGos = Utils.AllChildrenGameObjects(_status);
            _tickTimer = float.PositiveInfinity;
        }
    }
    Brain _br;
    
    public List<EffectGroup> _groups;
    float _tickTimer;
    const float CONST_TickMaxTime = 0.5f;

    void Update()
    {
        _tickTimer += Time.deltaTime;
        if (_tickTimer >= CONST_TickMaxTime)
        {
            _tickTimer = 0f;
        }
        
        int count = _groups.Count;
        for (int i = 0; i < count; i++)
        {
            EffectGroup g =  _groups[i];
            g.duration -= Time.deltaTime;
            if (g.duration > 0)
            {
                if (!g.IsDot() || !Mathf.Approximately(_tickTimer, 0f)) continue;
                Element element = Element.Physical;
                switch (g.effect)
                {
                    case Effect.Bleeding:
                        break;
                    case Effect.Burning:
                        element = Element.Fire;
                        break;
                    case Effect.Freezing:
                        element = Element.Ice;
                        break;
                    case Effect.Jolted:
                        element = Element.Electricity;
                        break;
                    case Effect.Poisoned:
                        element = Element.Poison;
                        break;
                }
                PassDataContainer passDataContainer = new PassDataContainer()
                {
                    myBrain = g.brain,
                    data = new List<PassData>()
                    {
                        new PassDataDamage(new MyDuo<Element, float>(new Element[1] { element }, new float[1] { g.intensity }))
                    }
                };
                Br.health.HealthInjectData(passDataContainer);
                continue;
            }
            Change(GenChange.Remove, g);
        }
    }
    void LateUpdate()
    {
        if (_groups.Count == 0) return;
       _status.position = Ga.me.camRig.cam.WorldToScreenPoint(Br.myTransform.position);
    }
    public void Change(GenChange addRemove, EffectGroup effectGroup)
    {
        print(addRemove);
        switch (addRemove)
        {
            case GenChange.Add:
                _groups.Add(effectGroup);
                break;
            case GenChange.Remove:
                if (_groups.Contains(effectGroup)) _groups.Remove(effectGroup);
                break;
        }
        Refresh();
    }


    void Refresh()
    {
        uIRefresh();
        return;
        
        void uIRefresh()
        {
            Utils.ActivateOneArrayElement(_statusImageGos);
            for (int i = 0; i < _groups.Count; i++)
            {
                _statusImageGos[(int)_groups[i].effect].SetActive(true);
            }
            
        }
    }

}
