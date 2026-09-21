using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class EnemyCombat : MonoBehaviour, IIniBrain, ITargetTracker
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
    }
    [SerializeField] WeaponSet[] weapons;
    [Button]
    void SortWeaponsByRange() =>  System.Array.Sort(weapons, (x, y) => x.spellMain.range.CompareTo(y.spellMain.range));

    
    public AnimAttackType? InAttackRange()
    {
        if (MyTarget == null) return null;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (Br.combat.distanceToTarget <= weapons[i].spellMain.range) return weapons[i].animAttackType;
        }
        return null;
    }
    SpellMain GetSpellByAttackType(AnimAttackType animAttackType)
    {
        foreach (WeaponSet item in weapons)
        {
            if (item.animAttackType == animAttackType) return item.spellMain;
        }
        return null;
    }
    float GetRangeByAttackType(AnimAttackType animAttackType)
    {
        foreach (WeaponSet item in weapons)
        {
            if (item.animAttackType == animAttackType) return item.spellMain.range;
        }
        return 0f;
    }
    [SerializeField] Transform spawnPoint;
    [SerializeField] EnemyLoco enemyLoco;

    public void AnimEv_AttackCallback(int num = 0)
    {
        switch (num)
        {
            case 0: //melee (used by treant, wolf and cobra)
                PassData containerMelee = new PassData()
                {
                    myBrain = Br,
                    canBeBlocked = true,
                    hasDamage = true,
                    damagePair = Br.character.GetDamage()
                };
                SpellMain melee = Instantiate(GetSpellByAttackType(AnimAttackType.Melee),
                    Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
                melee.areaOfEffect = 1.1f * GetRangeByAttackType(AnimAttackType.Melee);
                melee.InitializeMe(Br, containerMelee);
                break;
            case 1: //bullet (used by treant and cannon)
                PassData containerBullet = new PassData()
                {
                    myBrain = Br,
                    canBeBlocked = true,
                    hasDamage = true,
                    damagePair = Br.character.GetDamage(),
                    hasEffect =  true,
                    effects = new BuffEffects[1] { new BuffEffects(Status.Effect.Poisoned, 1, 3) }
                };
                Vector3 zeroSpawnPoint = new Vector3(spawnPoint.position.x, 0f, spawnPoint.position.z);
                SpellMain bullet = Instantiate(GetSpellByAttackType(AnimAttackType.Ranged), 
                    zeroSpawnPoint, Br.myTransform.rotation, Ga.me.spells.myTransform);
                BulletTransporter bulletTransporter = bullet.transporter as BulletTransporter;
                if (bulletTransporter != null)
                {
                    bulletTransporter.ricochet = (int)Br.character.GetStat(Stats.Ricochet);
                    bulletTransporter.pierce = (int)Br.character.GetStat(Stats.Piercing);
                    bulletTransporter.bounce = (int)Br.character.GetStat(Stats.Bounce);
                }
                bullet.visual.SetSpawnHeight(spawnPoint.position.y);
                bullet.InitializeMe(Br, containerBullet);
                break;
            case 2: //lob (used by scarecrow)
                SpellMain lob = Instantiate(GetSpellByAttackType(AnimAttackType.Ranged), 
                    spawnPoint.position, Quaternion.identity, Ga.me.spells.myTransform);
                PassData containerExplo = new PassData()
                {
                    myBrain = Br,
                    canBeBlocked = true,
                    hasDamage = true,
                    damagePair = Br.character.GetDamage()
                };
                lob.InitializeMe(Br, () =>
                {
                    SpellMain explosion = Instantiate(Ga.me.spells.explosionFire, lob.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                    explosion.InitializeMe(Br, containerExplo);
                });
                break;
            case 3: //dash with damage (used by cobra)
                SpellGroup.ComboDash(Br,  Br.character.GetDamage());
                break;
        }
    }

}


