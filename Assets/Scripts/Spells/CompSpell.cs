using UnityEngine;
using UnityEngine.Serialization;

public class CompSpell : MonoBehaviour
{
    public Transform myTransform;
    public Rigidbody myRigid;
    [FormerlySerializedAs("myCollider")] public SphereCollider mySphereCollider;
    public CapsuleCollider myCapsuleCollider;
    public Transform myMesh;

}
