using System.Collections.Generic;
using UnityEngine;


public class ProjectileTransporter : SpellTransporter
{
    enum  ProjType
    {
        Bullet,
        Homing
    }
    [SerializeField] ProjType projectileType;
    public override SpellMain Spell
    {
        get => base.Spell;
        set
        {
            base.Spell = value;
            value.myRigid.isKinematic = false;
            switch (projectileType)
            {
                case ProjType.Bullet:
                    SetSpeed();
                    break;
            }
        }
    }
    

    [SerializeField] float speed;
    
    void Update()
    {
        switch (projectileType)
        {
            case ProjType.Homing:
                if (!Spell.spellActive ) return;
                if (target == null) return;
                Vector3 dir = Utils.Direction(Spell.myTransform.position, target.position);
                Spell.myTransform.rotation = Quaternion.Slerp(Spell.myTransform.rotation, Quaternion.LookRotation(dir), speed * Time.deltaTime);
                break;
        }
    }
    
    
    void FixedUpdate()
    {
        switch (projectileType)
        {
            case ProjType.Homing:
                SetSpeed();
                break;
        }
    }

    void SetSpeed()
    {
        float sp = Spell.spellActive ? speed : 0f;
        Spell.myRigid.linearVelocity = sp * Spell.myTransform.forward;
    }

    public override bool CanRicochet(Collider other)
    {
        if (ricochet <= 0) return false;
        float range = 3f;
        Collider[] colliders = Physics.OverlapSphere(Spell.myTransform.position, range, Utils.MyLayer(Ga.me.gameData.layActors));
        List<Transform> myTargets = new List<Transform>();
        foreach (Collider item in colliders)
        {
            if (item == other) continue;
            if (item.TryGetComponent(out Brain ricochetTargetBrain) &&
                Utils.CanTargetFaction(Spell.OwnersBrain.Faction, ricochetTargetBrain.Faction, Spell.myFactionTarget))
            {
                myTargets.Add(item.transform);
            }
        }
        
        if (myTargets.Count > 0)
        {
            target = myTargets[Random.Range(0, myTargets.Count)];
            Vector3 dir = target.position - Spell.myTransform.position;
            ricochet--;
            Spell.myTransform.forward = dir.normalized;
            SetSpeed();
        }
        else return false;
        
        return base.CanRicochet(other);
    }
    public override bool CanBounce(Vector3 normal)
    {
        if (bounce <= 0) return false;
        bounce--;
        Vector3 dir = Vector3.Reflect(Spell.myTransform.forward, normal);
        Spell.myTransform.forward = dir.normalized;
        SetSpeed();

        return base.CanBounce(normal);
    }
    public override bool CanPierce()
    {
        if (pierce <= 0) return false;
        if (projectileType == ProjType.Homing)
        {
            projectileType = ProjType.Bullet;
            SetSpeed();
        }
        pierce--;
        
        return base.CanPierce();
    }

    public override void ReflectProjectile(Brain newBrain)
    {
        base.ReflectProjectile(newBrain);
        Spell.OwnersBrain = newBrain;
        Spell.myTransform.rotation *= Quaternion.Euler(0f, 180f, 0f);
        if (projectileType == ProjType.Homing)
        {
            projectileType = ProjType.Bullet;
        }

        SetSpeed();
    }

}
