using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class Utils
{
    public static Vector3 FrontSpawnPos(Transform myTransform, float range)
    {
        return myTransform.position + range * 0.5f * myTransform.forward + Vector3.up;
    }
    public static bool IsInLayerMask(GameObject go, LayerMask mask)
    {
        return (mask & (1 << go.layer)) != 0;
    }

    public static LayerMask LayHostiles(Faction faction)
    {
        switch (faction)
        {
            case Faction.Ally:
                return Ga.me.layEnemies;
            case Faction.Foe:
                return Ga.me.layPlayer;
            case Faction.Neutral:
                return 1;
            default:
                return default;
        }
    }
    public static Vector2 MakeV2(Vector3 v3) => new Vector2(v3.x, v3.z);
    public static Vector3 MakeV3(Vector2 v2, float height = 0f) => new Vector3(v2.x, height, v2.y);

    public static float Distance(Vector3 v1, Vector3 v2) => Vector2.Distance(MakeV2(v1), MakeV2(v2));

    public static Vector3 Direction(Transform trA, Transform trB)
    {
        return MakeV3(MakeV2(trB.position) - (MakeV2(trA.position)).normalized);
    }
    public static Vector3 Direction(Transform trA, Vector3 vecB)
    {
        return MakeV3(MakeV2(vecB) - (MakeV2(trA.position)).normalized);
    }
    public static Vector3 Direction(Vector3 vecA, Vector3 vecB)
    {
        return MakeV3(MakeV2(vecB) - (MakeV2(vecA)).normalized);
    }
    
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
        var rnd = new System.Random();
        var randNums = ints.OrderBy(n => rnd.Next());
        List<int> list = new List<int>();
        foreach (var item in randNums)
        {
            list.Add(item);
        }
        return list;
    }
    public static List<T> RandomListByType<T>(List<T> startList)
    {
        var rnd = new System.Random();
        var randNums = startList.OrderBy(n => rnd.Next());
        List<T> list = new List<T>();
        foreach (var item in randNums)
        {
            list.Add(item);
        }
        return list;
    }
    #endregion

}




