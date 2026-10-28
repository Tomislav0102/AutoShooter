using System;
using System.Collections;
using UnityEngine;

public class SpellShrine : MonoBehaviour, IIniSpell
{
    [SerializeField] SpellMain mySpell;
    public SpellMain Spell
    {
        get => _spell;
        set
        {
            _spell = value; 
            _bs = new BuffStats(myStat, BuffType.Added, _value, _duration);
            _spell.mySphereCollider.enabled = true;
        }
    }
    SpellMain _spell;
    public Stats myStat;
    int _duration = 20;
    int _value = 50;
    BuffStats _bs;

    void Start()
    {
        mySpell.InitializeMe(Ga.me.team.playersBrain);
    }

    public void OnEnter(Collider other)
    {
        if (!Spell.spellActive) return;
        if (other != Ga.me.team.playersBrain.myCollider) return;
        _spell.mySphereCollider.enabled = false;
        Spell.visual.StopDefault();
        Spell.spellActive = false;
        Ga.me.uiManager.StatsInject(GenChange.Add, myStat);
        Ga.me.team.playersBrain.character.CharacterInjectData(GenChange.Add, _bs);
        StartCoroutine(delay());
        return;

        IEnumerator delay()
        {
            yield return new WaitForSeconds(_duration);
            Ga.me.uiManager.StatsInject(GenChange.Remove, myStat);
        }
    }

}
