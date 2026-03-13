using System;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class WeaponHolster
{
    public Dictionary<GenOrder, Spell> spells;

    public WeaponHolster(GameObject go1, GameObject go2)
    {
        Spell first = go1.GetComponent<Spell>();
        Spell second = go2.GetComponent<Spell>();
        spells = new Dictionary<GenOrder, Spell>()
        {
            { GenOrder.Primary, first.myData.attackRange < second.myData.attackRange ? first : second },
            { GenOrder.Secondary, first.myData.attackRange < second.myData.attackRange ? second : first  }
        };
    }
}
public class Combat : SerializedMonoBehaviour, IInit
{
    public virtual Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            _searchWait = Random.Range(0f, 0.2f) + 0.5f;
            _targets = value.faction == Faction.Ally ? GameManager.Instance.team[Faction.Foe] : GameManager.Instance.team[Faction.Ally];
            _targetAim = transform.GetChild(0);
            StartCoroutine(SearchTargetCoroutine());

        }
    }
    Brain _br;
    protected GameManager gm;
    public bool IsReady { get; set; } //only called in children (because they're on scene)
    public bool isAttacking;
    [field: SerializeField] public virtual Transform MyTarget { get; set; }
    Transform _targetAim;
    protected float distance;
    [SerializeField] protected float detectRange = float.MaxValue;
    float _searchWait;
    HashSet<Transform> _targets;
    protected WeaponHolster holster;

    protected void Awake()
    {
        gm = GameManager.Instance;
    }


    protected virtual void Update()
    {
        if (MyTarget == null)
        {
            distance = -1;
            _targetAim.localPosition = Vector3.zero;
        }
        else
        {
            distance = Utils.Distance(Br.loco.myTransform.position, MyTarget.position);
            _targetAim.position = MyTarget.position;
        }
    }

    public virtual void FromAnimEv_Attack(int num = 0) { }

    IEnumerator SearchTargetCoroutine()
    {
        yield return new WaitForSeconds(_searchWait * 2);
        while (true)
        {
            CheckTarget();
            yield return new WaitForSeconds(_searchWait);
        }
    }

    void CheckTarget()
    { 
        if (MyTarget != null) return;
        MyTarget = Utils.ClosestTransform(Br.loco.myTransform.position, _targets, detectRange);
    }

}

[System.Serializable]
public struct WeaponData
{
    public float range;
    public float damage;
}
