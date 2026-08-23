using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class Drop : MonoBehaviour
{
    public DropType MyDrop
    {
        get => _myDrop;
        set
        {
            _myDrop = value;
            switch (value)
            {
                case DropType.Gold:
                    break;
                case DropType.Xp:
                    break;
                case DropType.Heal:
                    break;
                case DropType.ItemSpell:
                    break;
            }
            Transform targetGroup = parMeshes.GetChild((int)value);
            for (int i = 0; i < targetGroup.childCount; i++)
            {
               if (i == 0) targetGroup.GetChild(i).GetComponent<ParticleSystem>().Play();
               else  targetGroup.GetChild(i).GetComponent<ParticleSystem>().Stop();
            }
        }
    }
    [ShowInInspector, ReadOnly] DropType _myDrop;
    [SerializeField, EnumButtons] DropType startingDrop;
    bool IsHealType() => startingDrop == DropType.Heal;
    enum HealType { Meat, WeakHeal, StrongHeal}
    [SerializeField, EnumButtons, ShowIf(nameof(IsHealType))] HealType healType;
    [SerializeField] Rigidbody myRigid;
    [SerializeField] Transform parMeshes;
    [SerializeField] ParticleSystem psTrail;
    bool _ready;
    int _attractDistance = 10;
    float _pickUpDistance = 1.1f;
    Transform _myTransform;
    float _speed = 0.5f;

    IEnumerator Start()
    {
      //  MyDrop = startingDrop;
      _myTransform = transform;
        int rdn = System.Enum.GetNames(typeof(DropType)).Length;
        MyDrop = (DropType)Random.Range(0, rdn);
        JumpStart();
        yield return Ga.me.wait20;
        _ready = true;
    }

    [Button]
    void JumpStart()
    {
        Quaternion targetRot = Quaternion.Euler(-60f, Random.Range(0f, 360f), 0f);
        _myTransform.rotation = targetRot;
        parMeshes.rotation = Quaternion.Inverse(targetRot);
        myRigid.AddForce(10 * _myTransform.forward, ForceMode.Impulse);
    }

    void Update()
    {
        if (!_ready) return;
        float distance = Utils.Distance(Ga.me.team.playerTransform.position, _myTransform.position);
        
        if (distance > _attractDistance) return;
        Vector3 direction = Utils.Direction(_myTransform.position, Ga.me.team.playerTransform.position).normalized;
        _speed *= 1.01f;
        myRigid.AddForce(_speed * direction);
        
        if (distance > _pickUpDistance) return;
        _ready = false;
        StartCoroutine(EndDelay());
        
    }

    IEnumerator EndDelay()
    {
        parMeshes.gameObject.SetActive(false);
        GetComponent<Collider>().enabled = false;
        myRigid.isKinematic = true;
        yield return Ga.me.wait20;
        Destroy(gameObject);
    }
}
