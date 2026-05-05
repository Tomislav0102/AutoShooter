using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class Utils
{
    public static bool CanTargetFaction(Faction myFaction, Faction target, FactionToTarget targetFaction)
    {
        switch (targetFaction)
        {
            case FactionToTarget.Ally:
                return myFaction == target;
            case FactionToTarget.Enemy:
                return myFaction != target;
            case FactionToTarget.All:
                return true;
        }
        return false;
    }
    
    public static int MyLayer(string layerName)
    {
        int lay = LayerMask.NameToLayer(layerName);
        return  (1 << lay);
    }
    public static int MyLayers(string[] layerNames)
    {
        int lay = LayerMask.NameToLayer(layerNames[0]);
        int result = (1 << lay);

        if (layerNames.Length <= 1) return result;
        for (int i = 1; i < layerNames.Length; i++)
        {
            result |= (1 << LayerMask.NameToLayer(layerNames[i]));
        }

        return result;
        // layerMask = (1 << layer); //set layer a layermask
        // layerMask |= (1 << layer); //add layer to layermask
        // layerMask &= ~(1 << layer); //remove layer from layermask
    }


    public static Vector2 MakeV2(Vector3 v3) => new Vector2(v3.x, v3.z);
    public static Vector3 MakeV3(Vector2 v2, float height = 0f) => new Vector3(v2.x, height, v2.y);

    public static float Distance(Vector3 v1, Vector3 v2) => Vector2.Distance(MakeV2(v1), MakeV2(v2));
    public static Vector3 Direction(Vector3 fromPos, Vector3 toPos) => MakeV3(MakeV2(toPos) - (MakeV2(fromPos))).normalized;
    
    public static Transform ClosestTransform(Vector3 fromPosition, HashSet<Transform> targets, float maxRange = float.MaxValue)
    {
        Transform closest = null;
        float currentDistance = maxRange;
        Vector2 from2d = MakeV2(fromPosition);
        foreach (Transform item in targets)
        {
            float distance = Vector2.Distance(from2d, MakeV2(item.position));
            if (distance < currentDistance)
            {
                closest = item;
                currentDistance = distance;
            }
        }
        return closest;
    }
    public static Vector3 GetRandomPosition(Transform horizontalSurface)
    {
        float width = horizontalSurface.localScale.x * 0.5f;
        float length = horizontalSurface.localScale.z * 0.5f;
        float x = horizontalSurface.position.x + UnityEngine.Random.Range(-width, width);
        float z = horizontalSurface.position.z + UnityEngine.Random.Range(-length, length);
        return new Vector3(x, 0f, z);
    }

    
    public static void CameraFollowAsymptotic(Vector3 targetPos, Transform camTr)
    {
        //  const float CONST_CamEdgeBottom = 7f;

        float mod = 0.05f;
        float x = camTr.position.x;
        float z = camTr.position.z;
        
        x += (targetPos.x - x) * mod;
        z += (targetPos.z - z) * mod;
        camTr.position = new Vector3(x, camTr.position.y, z);
    }

    #region NOT USED
    public static bool IsInLayerMask(GameObject go, LayerMask mask)
    {
        return (mask & (1 << go.layer)) != 0;
    }

    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform rectElement) //sets gameobject behind the UI element
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectElement, rectElement.position, null, out Vector3 result);
        return result;
    }
    
    public static IEnumerator PositionRects(RectTransform rectLeft, RectTransform rectRight)
    {
        yield return new WaitForEndOfFrame();
        //any textMeshPro.rectTransform needs to have ContentSizeFitter component
        float xOffset = (rectRight.sizeDelta.x + rectLeft.sizeDelta.x) * 0.5f ;
        Vector2 newPos = new Vector2(rectRight.anchoredPosition.x - xOffset - 30, rectRight.anchoredPosition.y);
        rectLeft.anchoredPosition = newPos;
    }

    public static string ThousandsSeparator(int num) => num.ToString("N0").Replace(',', '.');
    
    public static IEnumerator CheckInternetConnection(System.Action<bool> isConnected)
    {
        UnityWebRequest request = new UnityWebRequest("https://google.com");
        yield return request.SendWebRequest();
        if (request.error == null)
        {
            // Debug.Log("success, connected to internet");
            isConnected?.Invoke(true);
        }
        else
        {
            // Debug.Log("no internet connection");
            isConnected?.Invoke(false);
        }
    }
    
    public static void Activation<T>(T tip, GenActivation activation)
    {
        if (tip == null) return;
    
        switch (tip)
        {
            case GameObject go:
            {
                if (go.activeSelf != (activation == GenActivation.On)) go.SetActive(activation == GenActivation.On);
                break;
            }
            case Transform tr:
            {
                if (tr.gameObject.activeSelf != (activation == GenActivation.On)) tr.gameObject.SetActive(activation == GenActivation.On);
                break;
            }
            case Image img:
                img.enabled = activation == GenActivation.On;
                break;
        }
    }
    public static GameObject[] AllChildrenGameObjects(Transform parGos)
    {
        GameObject[] gos = new GameObject[parGos.childCount];
        for (int i = 0; i < gos.Length; ++i)
        {
            gos[i] = parGos.GetChild(i).gameObject;
        }
        return gos;
    }
    public static T[] AllChildren<T>(Transform parTransform) where T : Component
    {
        T[] children = new T[parTransform.childCount];
        for (int i = 0; i < children.Length; ++i)
        {
            children[i] = parTransform.GetChild(i).GetComponent<T>();
        }
        return children;
    }
    public static void ActivateOneArrayElement<T>(T[] arr, int ordinal = System.Int32.MaxValue)
    {
        if (arr == null) return;
        for (int i = 0; i < arr.Length; i++)
        {
            Activation(arr[i], GenActivation.Off);
        }
        if (ordinal < arr.Length) Activation(arr[ordinal], GenActivation.On);
    }
    public static List<int> RandomList(int size)
    {
        List<int> ints = Enumerable.Range(0, size).ToList();
        return RandomListByType(ints);
    }
    public static List<T> RandomListByType<T>(List<T> startList)
    {
        int count = startList.Count;
        List<T> result = new List<T>(count);
        for (int i = 0; i < count; i++)
        {
            T t = startList[Random.Range(0, startList.Count)];
            result.Add(t);
            startList.Remove(t);
        }
        return  result;
        
        System.Random rnd = new System.Random(); 
        IOrderedEnumerable<T> randNums = startList.OrderBy(n => rnd.Next());
        return randNums.ToList();
    }
    #endregion

}




