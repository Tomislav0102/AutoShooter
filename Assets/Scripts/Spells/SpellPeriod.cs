using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class SpellPeriod : MonoBehaviour, IIniSpell
{
    public SpellMain Spell
    {
        get => _spell;
        set
        {
            _spell = value;
            _timerPeriod = Mathf.Infinity;
            _isRunning = true;
        }
    }
    SpellMain _spell;

    [SerializeField] UnityEvent onHit;
    [InfoBox("If value == 0, then its a one-hitter"), SerializeField] float rateOfFire;
    float _timerPeriod;
    bool _isRunning;


    void Update()
    {
        if (!_isRunning) return;
        if (Spell != null)
        {
            if (!Spell.spellActive) return;
            if (Spell.MyPhase != SpellMain.Phase.SpellRuns) return;
        }

        if (rateOfFire == 0)
        {
            onHit.Invoke();
            _isRunning = false;
            return;
        }

        _timerPeriod += Time.deltaTime;
        if (_timerPeriod <= rateOfFire) return;

        _timerPeriod = 0f;
        onHit.Invoke();
    }


}
