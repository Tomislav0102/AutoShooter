using Sirenix.OdinInspector;
using UnityEngine;

public class P_Mage : PlayerCombat
{
    [Title("Mage")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] int numOfHomingMissiles;
    public override Brain Br
    {
        get => base.Br;
        set
        {
            base.Br = value;
            value.loco.lookAtTarget = false;
        }
    }


    public override void FromAnimEv_Attack(int num = 0)
    {
        base.FromAnimEv_Attack(num);

        float angle = 180f / (numOfHomingMissiles + 1);
        for (int i = 0; i < numOfHomingMissiles; i++)
        {
            S_Homing spell = Instantiate(Ga.me.spells.homing, Ga.me.spells.myTransform).GetComponent<S_Homing>();
            spell.homingTarget = Br.combat.MyTarget;
            spell.myTransform.SetPositionAndRotation(Br.myTransform.position, Br.myTransform.rotation);
            spell.myTransform.rotation *= Quaternion.Euler(0f, -angle * (numOfHomingMissiles - 1) , 0f); //need fix
            Quaternion rotMultiplier = Quaternion.Euler(0f, angle * i, 0f);
            spell.myTransform.rotation *= rotMultiplier;
            spell.myMesh.position = new Vector3(spell.myMesh.position.x, spawnPoint.position.y, spell.myMesh.position.z);
            spell.InitializeMe(Br);
        }
    }
}
