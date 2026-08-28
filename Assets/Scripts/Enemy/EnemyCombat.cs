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
                PassDataContainer containerMelee = new PassDataContainer()
                {
                    myBrain = Br,
                    canBeBlocked = true,
                    data = new List<PassData>()
                    {
                        new PassDataDamage(new Element[1] { Element.Physical }, new float[1] { Br.character.GetStat(Stats.MeleeDamage) }),
                       // new PassDataKnockBack(5)
                    }
                };
                SpellMain melee = Instantiate(GetSpellByAttackType(AnimAttackType.Melee),
                    Br.myTransform.position, Br.myTransform.rotation, Ga.me.spells.myTransform);
                melee.areaOfEffect = 1.1f * GetRangeByAttackType(AnimAttackType.Melee);
                melee.InitializeMe(Br, containerMelee);
                break;
            case 1: //bullet (used by treant and cannon)
                PassDataContainer containerBullet = new PassDataContainer()
                {
                    myBrain = Br,
                    canBeBlocked = true,
                    data = new List<PassData>()
                    {
                        new PassDataDamage(new Element[1] { Element.Poison }, new float[1] { Br.character.GetStat(Stats.RangedDamage) }),
                    }
                };
                Vector3 zeroSpawnPoint = new Vector3(spawnPoint.position.x, 0f, spawnPoint.position.z);
                SpellMain bullet = Instantiate(GetSpellByAttackType(AnimAttackType.Ranged), 
                    zeroSpawnPoint, Br.myTransform.rotation, Ga.me.spells.myTransform);
                BulletTransporter bulletTransporter = bullet.transporter as BulletTransporter;
                if (bulletTransporter != null)
                {
                    bulletTransporter.ricochet = Br.character.GetStat(Stats.Ricochet);
                    bulletTransporter.pierce = Br.character.GetStat(Stats.Piercing);
                    bulletTransporter.bounce = Br.character.GetStat(Stats.Bounce);
                }
                bullet.visual.SetSpawnHeight(spawnPoint.position.y);
                bullet.InitializeMe(Br, containerBullet);
                break;
            case 2: //lob (used by scarecrow)
                SpellMain lob = Instantiate(GetSpellByAttackType(AnimAttackType.Ranged), 
                    spawnPoint.position, Quaternion.identity, Ga.me.spells.myTransform);
                PassDataContainer containerExplo = new PassDataContainer()
                {
                    myBrain = Br,
                    canBeBlocked = true,
                    data = new List<PassData>()
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
            case 3: //dash with damage (used by cobra)
                PassDataContainer pdDash = new PassDataContainer()
                {
                    data = new List<PassData>()
                    {
                        new PassDataDash(Ga.me.gameData.dashPower),
                    }
                };
                SpellMain dash = Instantiate(GetSpellByAttackType(AnimAttackType.Ranged), Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                dash.transporter.target = Br.myTransform;
                dash.lifeTime = Ga.me.gameData.dashTime;
                dash.InitializeMe(Br, pdDash);
                Vector2 knockBackDir2 = Utils.MakeV2(Br.myTransform.forward);
                knockBackDir2.Normalize();
                knockBackDir2 = Utils.RotateV2(knockBackDir2, 45f * (2 * Random.Range(0,2) - 1));
                PassDataContainer pdContactDamage = new PassDataContainer()
                {
                    data = new List<PassData>()
                    {
                        new PassDataDamage(new Element[1] { Element.Physical }, new float[1] { 5f }),
                        new PassDataKnockBack(30, knockBackDir2)
                    }
                };
                SpellMain contactDam = Instantiate(Ga.me.spells.contactDamage, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                contactDam.transporter.target = Br.myTransform;
                contactDam.lifeTime = Ga.me.gameData.dashTime;
                contactDam.areaOfEffect = 1.3f * Br.size;
                contactDam.InitializeMe(Br, pdContactDamage);
                break;
        }
    }

}


