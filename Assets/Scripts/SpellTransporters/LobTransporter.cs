using UnityEngine;

/// <summary>
/// Rigidbody needs to have constraints removed (so it can move vertically)
/// </summary>
public class LobTransporter : SpellTransporter
{
    Vector3 _rndRot;
    public override void InitializeMe(SpellControl spellControl)
    {
        base.InitializeMe(spellControl);
        main.myTransform.rotation *= Quaternion.Euler(-45f, Random.Range(0f, 360f), 0);
        main.myRigid.isKinematic = false;
        main.myRigid.useGravity = true;
        main.myRigid.AddForce(10 * Random.Range(0.6f, 1.4f) * main.myTransform.forward, ForceMode.VelocityChange);
        _rndRot = 500f * Random.insideUnitSphere;
    }
    
    void Update()
    {
        main.myMesh.Rotate(Time.deltaTime * _rndRot);
        if (!Ga.me.LevelMan.InsideLevel(main.myTransform.position)) 
        {
            main.onEnd?.Invoke();
        }
    }

}
