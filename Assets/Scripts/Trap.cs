using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Trap : MonoBehaviour, IIniBrain
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            value.myCollider.enabled = true;
            value.myCollider.radius = value.size * 0.5f;
            OvrPassData ovr = GetComponent<OvrPassData>();
            if (ovr != null) _pd = ovr.GetContainer(); 
            switch (activationType)
            {
                case ActivationType.Constant:
                    value.myCollider.enabled = false;
                    InstantiateSpell();
                    break;
                case ActivationType.OnTriggerOnce:
                    break;
                case ActivationType.OnTriggerRepeatedly:
                    break;
            }
        }
    }
    Brain _br;
    //overrides OvrPassData on Spell. If both overrides are missing than there is no PassData (which could be ok in some edge cases)
    [ShowInInspector, ReadOnly] PassDataContainer _pd;
    enum ActivationType
    {
        Constant,
        OnTriggerOnce,
        OnTriggerRepeatedly,
    }
    [SerializeField] ActivationType activationType;
    [SerializeField] bool sizeOfTrapDefinesSizeOfSpell;
    [SerializeField] SpellMain spellPrefab;
    [InfoBox("Used for particles/animations of trap mesh (not the spell)"), SerializeField] UnityEvent onHit;

    void InstantiateSpell()
    {
        SpellMain spellInstantiated = Instantiate(spellPrefab, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        if (sizeOfTrapDefinesSizeOfSpell) spellInstantiated.areaOfEffect = Br.size;
        if (_pd == null) spellInstantiated.InitializeMe(Br);
        else spellInstantiated.InitializeMe(Br, _pd);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Brain brain)) return;
        if (!Utils.CanTargetFaction(Br.Faction, brain.Faction, FactionToTarget.Enemy)) return;
        InstantiateSpell();
        if (activationType == ActivationType.OnTriggerOnce) Br.myCollider.enabled = false;
        onHit.Invoke();
    }
}
