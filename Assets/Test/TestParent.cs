using System.Collections;
using UnityEngine;

public class TestParent : MonoBehaviour
{
    public virtual IEnumerator Delay()
    {
        print("Before delay");
        yield return null;
        print("after delay");
    }
}
