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
    public Brain MyTarget
    {
        get => _myTarget;
        set
        {
            _myTarget = value;
            enemyLoco.behCurrent = value == null ? enemyLoco.behIdlingDefault: enemyLoco.behFightingDefault;
        }
    }
    Brain _myTarget;

    
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
                PassData pdMelee = new PassData()
                {
                    hasDamage = true,
                    damagePair = Br.character.GetDamage()
                };
                SpellMain melee = SpellMain.Sp(GetSpellByAttackType(AnimAttackType.Melee), Br, true);
                melee.areaOfEffect = 1.1f * GetRangeByAttackType(AnimAttackType.Melee);
                melee.InitializeMe(Br, pdMelee);
                break;
            case 1: //bullet (used by treant and cannon)
                PassData pdBullet = new PassData()
                {
                    hasDamage = true,
                    damagePair = Br.character.GetDamage(),
                    // hasEffect =  true,
                    // effects = new BuffEffects[1] { new BuffEffects(Status.Effect.Poisoned, 1, 20) }
                };
                SpellMain bullet = SpellMain.Sp(GetSpellByAttackType(AnimAttackType.Ranged), Utils.LevelV3(spawnPoint.position), Br.myTransform.rotation);
                bullet.transporter.ricochet = (int)Br.character.GetStat(Stats.Ricochet);
                bullet.transporter.pierce = (int)Br.character.GetStat(Stats.Piercing);
                bullet.transporter.bounce = (int)Br.character.GetStat(Stats.Bounce);
                bullet.visual.SetSpawnHeight(spawnPoint.position.y);
                bullet.InitializeMe(Br, pdBullet);
                break;
            case 2: //lob (used by scarecrow)
                SpellMain lob = SpellMain.Sp(GetSpellByAttackType(AnimAttackType.Ranged), spawnPoint.position);
                PassData pdExplosion = new PassData()
                {
                    hasDamage = true,
                    damagePair = Br.character.GetDamage()
                };
                lob.InitializeMe(Br, () =>
                {
                    SpellMain explosion = SpellMain.Sp(Ga.me.spells.explosionFire, Utils.LevelV3(lob.myTransform.position));
                    explosion.visual.SetSpawnHeight(lob.myTransform.position.y);
                    explosion.InitializeMe(Br, pdExplosion);
                });
                break;
            case 3: //dash with damage (used by cobra)
                SpellGroup.ComboDash(Br,  Br.character.GetDamage());
                break;
        }
    }

}


