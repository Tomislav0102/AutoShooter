using System.Collections;
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


    public override void InitializeMe(Brain ownersBrain)
    {
        base.InitializeMe(ownersBrain);
        float[] angles = Utils.RadialSpreadAngles(mainPrefabPair.Length());
        spellsRunning = new SpellMain[mainPrefabPair.Length()];
        for (int i = 0; i < mainPrefabPair.Length(); i++)
        {
            mainPrefabPair.GetValue(i).myBrain = ownersBrain;
            SpellMain spell = Instantiate(mainPrefabPair.GetKey(i), myTransform.position, Quaternion.identity, myTransform);
            spell.myTransform.localRotation = Quaternion.AngleAxis(angles[i], Vector3.up);
            spell.myTransform.position += distanceFromAnchor * spell.myTransform.forward;
            spell.InitializeMe(ownersBrain, mainPrefabPair.GetValue(i));
            spell.onHitTarget += (Brain br) =>
            {
                StartCoroutine(delay(spell));
            };
            spellsRunning[i] = spell;
        }

        IEnumerator delay(SpellMain spell)
        {
            spell.spellActive = false;
            spell.visual.StopDefault();
            yield return new WaitForSeconds(10);
            spell.spellActive = true;
            spell.visual.PlayDefault();
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
