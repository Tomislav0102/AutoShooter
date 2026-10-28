using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Archer : MonoBehaviour
{
    public enum Charge
    {
        Fire,
        Ice,
        Electricity,
        Magic,
        Explosion,
        Split,
        Hail
    }
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
            Br.skills.onSkillIncrease += SkillIncreaseCallback;
            _chargePair = new MyDuo<Charge, SoSkill>();
            int length = System.Enum.GetNames(typeof(Charge)).Length;
            for (int i = 0; i < length; i++)
            {
                _chargePair.Add((Charge)i,  null);
            }
        }
    }
    Brain _br;

    [SerializeField] Transform spawnPoint;
    [SerializeField] bool front;
    [SerializeField] bool diagonal, side, back;
    [SerializeField][Range(0, 3)] int followUp;
    const float CONST_HorGapBetweenProjectiles = 0.3f;
    Coroutine _rollCoroutine;
    const float CONST_RollTime = 0.3f;
    [SerializeField] ParticleSystem psRoll;
    SoSkill[] _allSkills;

    #region DATA
    MyDuo<Charge, SoSkill> _chargePair;
    #endregion

    void OnEnable()
    {
        Ga.OnUltimateActivated += CallEv_OnUltimateActivated;
    }
    void OnDisable()
    {
        Ga.OnUltimateActivated -= CallEv_OnUltimateActivated;
        Br.skills.onSkillIncrease -= SkillIncreaseCallback;
    }

    void SkillIncreaseCallback(SoSkill newSkill)
    {
        switch (newSkill.skillId)
        {
            case SkillId.ChargeFire:
                _chargePair.SetValueByKey(Charge.Fire, newSkill);
                break;
            case SkillId.ChargeIce:
                _chargePair.SetValueByKey(Charge.Ice, newSkill);
                break;
            case SkillId.ChargeExplode:
                _chargePair.SetValueByKey(Charge.Explosion, newSkill);
                break;
        }
    }
    public void CombatEventCallback(CombatEvent combatEvent, Brain otherBrain = null, SpellMain.Specialty specialty = SpellMain.Specialty.General)
    {
        
    }
    //archers Ultimate is not triggered by animation event
    void CallEv_OnUltimateActivated()
    {
        // if (_rollCoroutine != null) StopCoroutine(_rollCoroutine);
        // _rollCoroutine = StartCoroutine(RollCoroutine());
        // //need fix, replace Br.myRigid.AddForce with Br.agent.velocity
        // IEnumerator RollCoroutine()
        // {
        //     // Br.loco.OvrMove = true;
        //     // Br.loco.OvrOrientation = true;
        //     Br.loco.Roll();
        //     psRoll.Play();
        //     Vector3 dir = playerLoco.effJoystickValue == Vector2.zero ? Br.myTransform.forward : Utils.MakeV3(playerLoco.effJoystickValue);
        //     Br.myRigid.AddForce(80 * dir, ForceMode.VelocityChange);
        //     yield return new WaitForSeconds(CONST_RollTime);
        //     // Br.loco.OvrMove = false;
        //     // Br.loco.OvrOrientation = false;
        //     psRoll.Stop();
        // }

    }
    public void AnimEv_AttackCallback(int num = 0)
    {
        MyDuo<Element, float> dam = new MyDuo<Element, float>();
        HashSet<Charge> chargesThisShot = new HashSet<Charge>(); //defines charges in this shot, while '_chargePair' defines all charges player has

        SoSkill skillFire = _chargePair.GetValueByKey(Charge.Fire);
        if (skillFire != null && Random.value < skillFire.valueGeneric)
        {
            dam.AddRange(skillFire.passData.damagePair);
            chargesThisShot.Add(Charge.Fire);
        }
        SoSkill skillIce = _chargePair.GetValueByKey(Charge.Ice);
        if (skillIce != null && Random.value < skillIce.valueGeneric)
        {
            dam.AddRange(skillIce.passData.damagePair);
            chargesThisShot.Add(Charge.Ice);
        }

        PassData pdBase = new PassData()
        {
            hasDamage = true,
            damagePair = Br.character.GetDamage(Element.Physical, 1f, dam)
        };
        SpellMain sp = SpellMain.Sp(Br.skills.myBasic.spell, Br, true);
        sp.myTransform.position = Utils.LevelV3(spawnPoint.position);
        sp.visual.SetSpawnHeight(spawnPoint.position.y);
        sp.visual.GetComponent<ArcherCharges>().InitializeMe(chargesThisShot);
        sp.InitializeMe(Br, pdBase, () =>  onAfterSpell(chargesThisShot));
        return;

        void onAfterSpell(HashSet<Charge> charges)
        {
            if (charges is null || charges.Count == 0) return;
            foreach (Charge item in charges)
            {
                switch (item)
                {
                    case Charge.Fire:
                        PassData passData = new PassData()
                        {
                            hasEffect = true,
                            effects = skillFire.passData.effects
                        };
                        SpellMain spell = SpellMain.Sp(skillFire.spell, Br);
                        spell.myTransform.position = sp.myTransform.position;
                        spell.areaOfEffect += skillFire.passData.stats[0].value;
                        spell.InitializeMe(Br, passData);
                        break;
                    case Charge.Ice:
                        break;
                }
            }
        }
    }

    #region OLD
    // Shoot_Old();
    // if (followUp > 0) StartCoroutine(ShootFollowUp());
    IEnumerator ShootFollowUp()
    {
        for (int i = 0; i < followUp; i++)
        {
            yield return Ga.me.wait01;
            Shoot_Old();
        }
    }
    void Shoot_Old()
    {
        if (front)
        {
            spawnProjectile(0f);
        }
        if (back)
        {
            spawnProjectile(180f);
        }
        if (diagonal)
        {
            for (int i = 0; i < 2; i++)
            {
                spawnProjectile(45 * (i * 2 - 1));
            }
        }
        if (side)
        {
            for (int i = 0; i < 2; i++)
            {
                spawnProjectile(90 * (i * 2 - 1));
            }
        }
        
        Vector3 fw = Br.myTransform.forward;
        float angle = Mathf.Atan2(fw.x, fw.z) * Mathf.Rad2Deg;
        spawnProjectile(angle);
        
        void spawnProjectile(float rotation)
        {
            Vector3 rot = rotation * Vector3.up;
            PassData container = new PassData()
            {
                hasDamage = true,
                damagePair = Br.character.GetDamage(new Element[2] { Element.Physical , Element.Electricity})
            };
            int projectile = (int)Br.character.GetStat(Stats.Projectiles);
            for (int i = 0; i < projectile; i++)
            {
                float xOffset = i * CONST_HorGapBetweenProjectiles;
                SpellMain sp = SpellMain.Sp(Br.skills.myBasic.spell, Br);
                sp.visual.SetSpawnHeight(spawnPoint.position.y);
                sp.myTransform.Rotate(rot);
                sp.myTransform.Translate(xOffset * Vector3.right, Space.Self);
                float width = projectile * CONST_HorGapBetweenProjectiles;
                sp.myTransform.Translate((width - CONST_HorGapBetweenProjectiles) * 0.5f * Vector3.left, Space.Self);
                sp.InitializeMe(Br, container);
            }
        }
    }
    #endregion

}
