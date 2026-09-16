using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class PC_Archer : MonoBehaviour
{
    public Brain Br
    {
        get => _br;
        set
        {
            _br = value;
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
    SoSkill _myBasic;
    SoSkill[] _allSkills;
    
    // public void SkillUpdate()
    // {
    //     _allSkills = Br.skills.CurrentSkills();
    //     foreach (SoSkill item in _allSkills)
    //     {
    //         switch (item.skillName)
    //         {
    //             case SkillName.ArcherBase:
    //                 _myBasic = item;
    //                 break;
    //             case SkillName.FrontArrow:
    //                 front = true;
    //                 break;
    //             case SkillName.DiagonalArrow:
    //                 diagonal = true;
    //                 break;
    //             case SkillName.SideArrow:
    //                 side = true;
    //                 break;
    //             case SkillName.BackArrow:
    //                 back = true;
    //                 break;
    //         }
    //     }
    // }

    void OnEnable()
    {
        EventBus.OnUltimateActivated += CallEv_OnUltimateActivated;
    }
    void OnDisable()
    {
        EventBus.OnUltimateActivated -= CallEv_OnUltimateActivated;
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
    public void CombatEventCallback(CombatEvent combatEvent, Brain otherBrain = null)
    {
        
    }
    public void AnimEv_AttackCallback(int num = 0)
    {
        Shoot();
        if (followUp > 0) StartCoroutine(ShootFollowUp());
    }

    IEnumerator ShootFollowUp()
    {
        for (int i = 0; i < followUp; i++)
        {
            yield return Ga.me.wait01;
            Shoot();
        }
    }

    void Shoot()
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
            PassDataContainer container = new PassDataContainer()
            {
                myBrain = Br,
                canBeBlocked = true,
                hasDamage = true,
                damagePair = Br.character.GetDamage(Stats.RangedDamage)
            };
            int projectile = (int)Br.character.GetStat(Stats.Projectiles);
            for (int i = 0; i < projectile; i++)
            {
                float xOffset = i * CONST_HorGapBetweenProjectiles;
                SpellMain sp = Instantiate(_myBasic.spell, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
                sp.visual.SetSpawnHeight(spawnPoint.position.y);
                sp.myTransform.Rotate(rot);
                sp.myTransform.Translate(xOffset * Vector3.right, Space.Self);
                float width = projectile * CONST_HorGapBetweenProjectiles;
                sp.myTransform.Translate((width - CONST_HorGapBetweenProjectiles) * 0.5f * Vector3.left, Space.Self);
                sp.InitializeMe(Br, container);
            }
        }
    }
}
