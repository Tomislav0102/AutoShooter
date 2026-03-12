using System;
using UnityEngine;

public class P_Knight : PlayerCombat
{

    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);
        GameObject go = Instantiate(GameManager.Instance.spellManager.melee, 
            Utils.FrontSpawnPos(Br.loco.myTransform, GameManager.Instance.spellManager.melee.GetComponent<S_Area>().myData.areaOfEffect), 
            Br.loco.myTransform.rotation);
        S_Area melee = go.GetComponent<S_Area>();
        melee.InitializeMe(GameManager.Instance.layEnemies, (Transform tr) =>
       {
         //  print($"{tr.gameObject.name} is hit");
       });
    }
    
    protected override void CallEv_OnSpecialActivated()
    {
        base.CallEv_OnSpecialActivated();
        StartCoroutine(Br.loco.Dash());
    }
}
