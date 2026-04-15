using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class P_Barb : PlayerCombat
{
    [SerializeField] Transform weaponTr;
    float _weaponAngle;



    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);
        // _weaponAngle = Vector3.SignedAngle(weaponTr.forward, Br.myTransform.forward, Vector3.up);
        _weaponAngle = Vector2.SignedAngle(Utils.MakeV2(weaponTr.forward), Utils.MakeV2(Br.myTransform.forward));
       // print($"{num} |||| {_weaponAngle}");
        
        Spell melee = Instantiate(Ga.me.spells.meleePlayer, Br.myTransform.position, Quaternion.identity, Ga.me.spells.myTransform);
        Vector3 pos = melee.comp.myTransform.position +
                      melee.areaOfEffect * 1f * Vector3.ProjectOnPlane(weaponTr.forward, Vector3.up);
        melee.comp.myTransform.position = pos;
        melee.InitializeMe(Br);

    }
}
