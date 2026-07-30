using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class EnemyCombat : MonoBehaviour, IInitialization, ITargetTracker
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            if (weapons.Length > 0) SortWeaponsByRange();
        }
    }
    Brain _br;
    public Transform MyTarget
    {
        get => _myTarget;
        set
        {
            _myTarget = value;
            enemyLoco.behCurrent = value == null ? enemyLoco.behIdlingDefault: enemyLoco.behFightingDefault;
        }
    }
    Transform _myTarget;

    [System.Serializable]
    class WeaponSet
    {
        public SpellMain spellMain;
        public AnimAttackType animAttackType; 
        public float range; 
    }
    [SerializeField] WeaponSet[] weapons;
    [Button]
    void SortWeaponsByRange() =>  Array.Sort(weapons, (x, y) => x.range.CompareTo(y.range));

    
    public AnimAttackType? InAttackRange()
    {
        if (MyTarget == null) return null;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (Br.combat.distanceToTarget <= weapons[i].range) return weapons[i].animAttackType;
        }
        return null;
    }
    protected SpellMain GetSpellByAttackType(AnimAttackType animAttackType)
    {
        foreach (WeaponSet item in weapons)
        {
            if (item.animAttackType == animAttackType) return item.spellMain;
        }
        return null;
    }
    [SerializeField] Transform spawnPoint;
    [SerializeField] EnemyLoco enemyLoco;

    public void AnimEv_AttackCallback(int num = 0)
    {
        switch (num)
        {
            case 0: //melee
                PassDataContainer containerMelee = new PassDataContainer()
                {
                    myBrain = Br,
                    canBeBlocked = true,
                    data = new PassData[2]
                    {
                        new PassDataDamage(new Element[1] { Element.Physical }, new float[1] { Br.character.GetStat(Stats.MeleeDamage) }),
                        new PassDataKnockBack(5)
                    }
                };
                SpellMain melee = Instantiate(GetSpellByAttackType(AnimAttackType.Melee),
                    Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
                melee.InitializeMe(Br, containerMelee);
                break;
            case 1: //bullet
                PassDataContainer containerBullet = new PassDataContainer()
                {
                    myBrain = Br,
                    canBeBlocked = true,
                    data = new PassData[1]
                    {
                        new PassDataDamage(new Element[1] { Element.Poison }, new float[1] { Br.character.GetStat(Stats.RangedDamage) }),
                    }
                };
                Vector3 zeroSpawnPoint = new Vector3(spawnPoint.position.x, 0f, spawnPoint.position.z);
                SpellMain bullet = Instantiate(GetSpellByAttackType(AnimAttackType.Ranged), 
                    zeroSpawnPoint, Br.myTransform.rotation, Ga.me.spells.myTransform);
                bullet.visual.SetSpawnHeight(spawnPoint.position.y);
                bullet.InitializeMe(Br, containerBullet);
                break;
            case 2: //extra
                SpellMain lob = Instantiate(GetSpellByAttackType(AnimAttackType.Ranged), 
                    spawnPoint.position, Quaternion.identity, Ga.me.spells.myTransform);
                PassDataContainer containerExplo = new PassDataContainer()
                {
                    myBrain = Br,
                    canBeBlocked = true,
                    data = new PassData[1]
                    {
                        new PassDataDamage(new Element[1] { Element.Fire }, new float[1] { Br.character.GetStat(Stats.RangedDamage) }),
                    }
                };
                lob.InitializeMe(Br, () =>
                {
                    SpellMain explosion = Instantiate(Ga.me.spells.explosionFire, lob.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    explosion.InitializeMe(Br, containerExplo);
                });
                break;
        }
    }

}


