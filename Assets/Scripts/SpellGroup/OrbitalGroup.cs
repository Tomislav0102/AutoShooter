using UnityEngine;
using Sirenix.OdinInspector;


public class OrbitalGroup : SpellGroup
{
    [ReadOnly] public Transform orbitingAnchor;
    [SerializeField] float distanceFromAnchor;
    [SerializeField] bool followPosition = true;
    [SerializeField] bool followRotation = true;
    [SerializeField, HideIf(nameof(followRotation))] int rotationSpeed = 20;
    [SerializeField] bool fullCircle = true;
    [SerializeField, Range(0, 359), HideIf(nameof(fullCircle))] int arc = 359;


    public override void InitializeMe(Brain ownersBrain, MyDuo<SpellMain, PassDataContainer> duo)
    {
        base.InitializeMe(ownersBrain, duo);
        float[] angles = Utils.RadialSpreadAngles(duo.Length());
        for (int i = 0; i < duo.Length(); i++)
        {
            SpellMain spell = Instantiate(duo.GetKey(i), myTransform.position, Quaternion.identity, myTransform);
            spell.myTransform.localRotation = Quaternion.AngleAxis(angles[i], Vector3.up);
            spell.myTransform.position += distanceFromAnchor * spell.myTransform.forward;
            spell.InitializeMe(ownersBrain, duo.GetValue(i));
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
