using UnityEngine;
using Sirenix.OdinInspector;


public class OrbitalGroup : SpellGroup
{
    [ReadOnly] public Transform orbitingAnchor;
    public float distanceFromAnchor;
    [Range(1, 6)] public int numOfActiveSpells = 1;
    [SerializeField] int rotationSpeed = 20;

    public override void InitializeMe(Brain ownersBrain)
    {
        base.InitializeMe(ownersBrain);
        float angle = 360f / numOfActiveSpells;
        for (int i = 0; i < mySpells.Length; i++)
        {
            if (i >= numOfActiveSpells) return;
            
            SpellMain spellMain = mySpells[i];
            spellMain.gameObject.SetActive(true);
            spellMain.InitializeMe(ownersBrain);
            spellMain.myTransform.localRotation = Quaternion.Euler(0, angle * (i + 1), 0);
            spellMain.myTransform.position += distanceFromAnchor * spellMain.myTransform.forward;
        }

    }

    void Update()
    {
        myTransform.position = orbitingAnchor.position;
        myTransform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
    }
}
