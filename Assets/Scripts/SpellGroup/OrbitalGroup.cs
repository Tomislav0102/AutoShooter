using UnityEngine;
using Sirenix.OdinInspector;


public class OrbitalGroup : SpellGroup
{
    [ReadOnly] public Transform orbitingAnchor;
    public float distanceFromAnchor;
    public bool followPosition = true;
    public bool followRotation = true;
    [SerializeField, ShowIf(nameof(followRotation))] int rotationSpeed = 20;
    [Range(1, 6)] public int numOfActiveSpells = 1;
    [SerializeField] bool fullCircle = true;
    [SerializeField, Range(0, 359), HideIf(nameof(fullCircle))] int arc = 359;


    public override void InitializeMe(Brain ownersBrain)
    {
        base.InitializeMe(ownersBrain);
        Transform[] spells = new Transform[numOfActiveSpells];
        for (int i = 0; i < numOfActiveSpells; i++)
        {
            SpellMain current = mySpells[i];
            spells[i] = current.myTransform;
            current.gameObject.SetActive(true);
            current.InitializeMe(ownersBrain);
        }
        Utils.RadialSpread(spells, distanceFromAnchor, fullCircle, arc);
        // for (int i = 0; i < spells.Length; i++)
        // {
        //     SpellMain spellMain = mySpells[i];
        //     spellMain.gameObject.SetActive(true);
        //     spellMain.InitializeMe(ownersBrain);
        //     spellMain.myTransform.rotation = Quaternion.Euler(0, angle * (i + 1), 0);
        //     spellMain.myTransform.position += distanceFromAnchor * spellMain.myTransform.forward;
        // }

    }

    void Update()
    {
        if (!followPosition && !followRotation) return;
        
        if (followPosition) myTransform.position = orbitingAnchor.position;
        if (followRotation) myTransform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
        else myTransform.rotation = orbitingAnchor.rotation;
    }
}
