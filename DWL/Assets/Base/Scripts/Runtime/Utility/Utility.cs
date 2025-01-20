using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neofect.Utility
{
    public class Utility : MonoBehaviour
    {
        public static T FindChild<T>(GameObject go, string name = null, bool isRecursive = false) where T : UnityEngine.Object
        {
            if (null == go)
                return null;

            if (isRecursive == false)
            {
                for (int index = 0; index < go.transform.childCount; index++)
                {
                    Transform transform = go.transform.GetChild(index);
                    if (string.IsNullOrEmpty(name) || transform.name == name)
                    {
                        T component = transform.GetComponent<T>();
                        if (null != component)
                            return component;
                    }
                }
            }
            else
            {
                foreach (T component in go.GetComponentsInChildren<T>(true))
                {
                    if (string.IsNullOrEmpty(name) || component.name == name)
                        return component;
                }
            }

            return null;
        }

        public static GameObject FindChild(GameObject go, string name = null, bool isRecursive = false)
        {
            Transform transform = FindChild<Transform>(go, name, isRecursive);
            if (null == transform)
                return null;

            return transform.gameObject;
        }

        public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
        {
            T component = go.GetComponent<T>();
            if (null == component)
                component = go.AddComponent<T>();

            return component;
        }
    }
}