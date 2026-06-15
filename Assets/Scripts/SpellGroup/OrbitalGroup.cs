using UnityEngine;
using Sirenix.OdinInspector;


public class OrbitalGroup : SpellGroup
{
    [ReadOnly] public Transform orbitingAnchor;
    [SerializeField] float distanceFromAnchor;
    [SerializeField] bool followPosition = true;
    [SerializeField] bool followRotation = true;
    [SerializeField, ShowIf(nameof(followRotation))] int rotationSpeed = 20;
    [SerializeField] bool fullCircle = true;
    [SerializeField, Range(0, 359), HideIf(nameof(fullCircle))] int arc = 359;


    public override void InitializeMe(Brain ownersBrain)
    {
        base.InitializeMe(ownersBrain);
        InitializeMe_Shared(ownersBrain);
    }
    public override void InitializeMe(Brain ownersBrain, SpellMain[] spellsToAdd)
    {
        base.InitializeMe(ownersBrain, spellsToAdd);
        InitializeMe_Shared(ownersBrain);
    }

    void InitializeMe_Shared(Brain ownersBrain)
    {
        float[] angles = Utils.RadialSpreadAngles(myTransform.childCount);
        for (int i = 0; i < myTransform.childCount; i++)
        {
            SpellMain spell = mySpells[i];
            spell.gameObject.SetActive(true);
            spell.myTransform.localRotation = Quaternion.AngleAxis(angles[i], Vector3.up);
            spell.myTransform.position += distanceFromAnchor * spell.myTransform.forward;
            spell.InitializeMe(ownersBrain);
        }
    }

    void Update()
    {
        if (!followPosition && !followRotation) return;
        
        if (followPosition) myTransform.position = orbitingAnchor.position;
        if (followRotation) myTransform.rotation = orbitingAnchor.rotation;
        else myTransform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
    }
}
