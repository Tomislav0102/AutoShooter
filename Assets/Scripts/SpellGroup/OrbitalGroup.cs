using UnityEngine;
using Sirenix.OdinInspector;


public class OrbitalGroup : SpellGroup
{
    [ReadOnly] public Transform orbitingAnchor;
    public float distanceFromAnchor;
    [Range(1, 6)] public int numOfActiveSpells = 1;

    public override void InitializeMe(Brain ownersBrain)
    {
        base.InitializeMe(ownersBrain);
        float angle = 360f / numOfActiveSpells;
        for (int i = 0; i < mySpells.Length; i++)
        {
            if (i >= numOfActiveSpells) return;
            
            SpellControl spellControl = mySpells[i];
            spellControl.gameObject.SetActive(true);
            spellControl.InitializeMe(ownersBrain);
            spellControl.myTransform.localRotation = Quaternion.Euler(0, angle * (i + 1), 0);
            spellControl.myTransform.position += distanceFromAnchor * spellControl.myTransform.forward;
        }

    }

    void Update()
    {
        myTransform.position = orbitingAnchor.position;
        myTransform.Rotate(20f * Time.deltaTime * Vector3.up);
    }
}
