using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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
            _duoBuffs = new MyDuo<BuffEffects, float>();
            _status = Instantiate(Ga.me.uiManager.statusPrefab, Ga.me.uiManager.pointersContainer);
            _statusImageGos = Utils.AllChildrenGameObjects(_status);
            _tickTimer = float.PositiveInfinity;
        }
    }
    Brain _br;
    [ReadOnly, ShowInInspector] MyDuo<BuffEffects, float> _duoBuffs;
    float _tickTimer;
    const float CONST_TickMaxTime = 0.5f;


    void OnDestroy()
    {
      if (_status != null)  Destroy(_status.gameObject);
    }
    void Update()
    {
        if (_duoBuffs.Length() == 0) return;
        
        _tickTimer += Time.deltaTime;
        if (_tickTimer >= CONST_TickMaxTime)
        {
            _tickTimer = 0f;
        }

        List<BuffEffects> buffsToRemove = new List<BuffEffects>();
        for (int i = 0; i < _duoBuffs.Length(); i++)
        {
            BuffEffects buff = _duoBuffs.GetKey(i);
            float duration = _duoBuffs.GetValue(i);
            
            duration -= Time.deltaTime;
            _duoBuffs.SetValue(i, duration);
            if (duration <= 0f) buffsToRemove.Add(buff);
            else
            {
                if (!buff.IsDot() || !Mathf.Approximately(_tickTimer, 0f)) continue;
                Element element = Element.Physical;
                switch (buff.effect)
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
                PassData passData = new PassData()
                {
                    hasDamage = true,
                    damagePair = new MyDuo<Element, float>(new Element[1] { element }, new float[1] { buff.data.value })
                };
                Br.health.HealthInjectDataDamage(passData, out _);
            }
        }
        bool doRefresh = false;
        for (int i = 0; i < buffsToRemove.Count; i++)
        {
            if (_duoBuffs.HasKey(buffsToRemove[i]))
            {
                doRefresh = true;
                _duoBuffs.Remove(buffsToRemove[i]);
                break;
            }
        }
        if (doRefresh) Refresh();

    }
    void LateUpdate()
    {
        if (_duoBuffs.Length() == 0) return;
       _status.position = Ga.me.camRig.cam.WorldToScreenPoint(Br.myTransform.position);
    }
    
    public void StatusInjectData(GenChange change, BuffEffects buffEffects)
    {
        switch (change)
        {
            case GenChange.Add:
                _duoBuffs.Add(buffEffects, buffEffects.data.permanent ? float.PositiveInfinity : buffEffects.data.Duration);
                if (buffEffects.IsDot()) return;

                for (int i = 0; i < _duoBuffs.Length() - 1; i++)
                {
                    if (_duoBuffs.GetKey(i).effect != buffEffects.effect) continue;
                    float duration = _duoBuffs.GetValue(i) +  buffEffects.data.Duration;
                    _duoBuffs.SetValue(i, duration);
                    break;
                }

                switch (buffEffects.effect)
                {
                    case Effect.InstantKill:
                        if (Br.health.HealthCurrent <= buffEffects.data.value * Br.character.GetStat(Stats.Health) * 0.01f)
                        {
                            print("Executioner");
                            Br.combat.CombatEventRegistered(CombatEvent.GetHit, buffEffects.brain);
                            Br.health.Death(buffEffects.brain);
                            return;
                        }
                        break;
                }
                
                break;
            case GenChange.Remove:
                if (_duoBuffs.HasKey(buffEffects)) _duoBuffs.Remove(buffEffects);
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
            for (int i = 0; i < _duoBuffs.Length(); i++)
            {
                _statusImageGos[(int)_duoBuffs.GetKey(i).effect].SetActive(true);
            }
        }
    }

    public bool HasEffect(Effect effect)
    {
        for (int i = 0; i < _duoBuffs.Length(); i++)
        {
            if (_duoBuffs.GetKey(i).effect ==  effect) return true;
        }
        return false;
    }

}
