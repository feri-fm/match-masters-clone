using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIFilter : MonoBehaviour
{
    public GameObject[] validPointerObjects;

    public static UIFilter Instance { get { return FindObjectOfType<UIFilter>(); } }

    public static bool IsPointerClear()
    {
        return IsPointerClear(Instance.validPointerObjects);
    }
    public static bool IsPointerClear(GameObject[] aa)
    {
        Vector3[] ps = new Vector3[Input.touchCount + 1];
        ps[0] = Input.mousePosition;
        for (int k = 0; k < Input.touchCount; k++)
            ps[k + 1] = Input.touches[k].position;
        return IsPointerClear(ps, aa);
    }
    public static bool IsPointerClear(Vector3[] ps, GameObject[] aa)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        bool[] results = new bool[ps.Length];
        for (int k = 0; k < ps.Length; k++)
        {
            pointer.position = ps[k];
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            int c = 0;
            for (int i = 0; i < raycastResults.Count; i++)
            {
                for (int j = 0; j < aa.Length; j++)
                {
                    if (raycastResults[i].gameObject == aa[j])
                    {
                        c++;
                        break;
                    }
                }
            }
            results[k] = c != raycastResults.Count;
        }
        for (int i = 0; i < results.Length; i++)
            if (!results[i])
                return false;
        return true;
    }

    public static bool IsPointerOverGameObject(params GameObject[] targets)
    {
        Vector3[] ps = new Vector3[Input.touchCount + 1];
        ps[0] = Input.mousePosition;
        for (int k = 0; k < Input.touchCount; k++)
            ps[k + 1] = Input.touches[k].position;
        return IsPointerOverGameObject(ps, targets);
    }
    public static bool IsPointerOverGameObject(Vector3[] ps, GameObject[] targets)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        for (int k = 0; k < ps.Length; k++)
        {
            pointer.position = ps[k];
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            for (int i = 0; i < raycastResults.Count; i++)
            {
                for (int j = 0; j < targets.Length; j++)
                {
                    if (raycastResults[i].gameObject == targets[j])
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static bool[] CheckPointers(params GameObject[] targets)
    {
        return CheckPointers(GetCurrentPointers(), targets);
    }
    public static bool[] CheckPointers(Vector3[] pointers, GameObject[] targets)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        bool[] results = new bool[targets.Length];
        for (int k = 0; k < pointers.Length; k++)
        {
            pointer.position = pointers[k];
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            for (int i = 0; i < raycastResults.Count; i++)
            {
                for (int j = 0; j < targets.Length; j++)
                {
                    if (raycastResults[i].gameObject == targets[j])
                    {
                        results[j] = true;
                        break;
                    }
                }
            }
        }
        return results;
    }

    public static List<GameObject> GetCurrentTargets()
    {
        return GetCurrentTargets(GetCurrentPointers());
    }
    public static List<GameObject> GetCurrentTargets(Vector3[] pointers)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        List<GameObject> results = new List<GameObject>();
        for (int k = 0; k < pointers.Length; k++)
        {
            pointer.position = pointers[k];
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            for (int i = 0; i < raycastResults.Count; i++)
            {
                results.Add(raycastResults[i].gameObject);
            }
        }
        return results;
    }

    public static Vector3[] GetCurrentPointers()
    {
        Vector3[] pointers = new Vector3[Input.touchCount + 1];
        pointers[0] = Input.mousePosition;
        for (int k = 0; k < Input.touchCount; k++)
            pointers[k + 1] = Input.touches[k].position;
        return pointers;
    }
}
