using System;
using UnityEngine;

/// <summary>
/// Only on sphere colliders
/// </summary>
public class ArcCalculator : MonoBehaviour, IIniSpell
{
    public SpellMain Spell
    {
        get => _spell;
        set
        {
            _spell = value;
        }
    }
    SpellMain _spell;
    [SerializeField] float angle;
    [SerializeField] bool isInverse;
    
    
    public bool TargetInsideArc(Vector3 targetPos)
    {
        Vector2 targetDir = (Utils.MakeV2(targetPos) - Utils.MakeV2(Spell.myTransform.position)).normalized;
        float signedAngle = Vector2.SignedAngle(Utils.MakeV2(Spell.myTransform.forward), targetDir);
        if (isInverse)
        {
            return MathF.Abs(signedAngle) >= MathF.Abs(180 - angle * 0.5f);
        }
        return MathF.Abs(signedAngle) <= MathF.Abs(angle * 0.5f);
    }


}
