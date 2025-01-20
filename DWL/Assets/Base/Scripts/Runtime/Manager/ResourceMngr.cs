using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceMngr : MonoBehaviour, IMngr
{
    private Dictionary<int, GameObject> resourceDic;        // Addressable로 로드한 오브젝트 관리

    public ResourceMngr()
    {
        resourceDic = new Dictionary<int, GameObject>();
    }

    /// <summary>
    /// Asset을 Load하는 함수
    /// </summary>
    /// <param name="assetReference"> Asset의 주소</param>
    /// <param name="root"> Asset이 로드 후, instantiate 되었을 때의 부모 Transform</param>
    /// <param name="successCallback"> 로드 및 instantiate에 성공했을 때 불리는 함수</param>
    /// <param name="failCallback"> 로드 및 instantitate에 실패 했을 때 불리는 함수</param>
    public void Load(AssetReference assetReference, Transform root, System.Action<GameObject> successCallback, System.Action failCallback)
    {
        GameObject gameObj = null;
        int hashCode = assetReference.GetHashCode();
        if (resourceDic.TryGetValue(hashCode, out gameObj))
        {
            if (null != gameObj)
            {
                successCallback?.Invoke(gameObj);
            }
        }
        else
        {
            //GameObject prefab = Resources.Load<GameObject>($"Login.prefab");
            //if (null != prefab) 
            //{
            //    successCallback?.Invoke(Instantiate(prefab, root));
            //}
            StartCoroutine(LoadAsset(assetReference, root, successCallback, failCallback));
        }
    }

    /// <summary>
    /// Addressable Asset을 로드할 때 비동기 방식으로 로드 되기 때문에 코루틴을 사용
    /// </summary>
    /// <param name="assetReference"> Asset의 주소</param>
    /// <param name="root"> Asset이 로드 후, instantiate 되었을 때의 부모 Transform</param>
    /// <param name="successCallback"> 로드 및 instantiate에 성공했을 때 불리는 함수</param>
    /// <param name="failCallback"> 로드 및 instantitate에 실패 했을 때 불리는 함수</param>
    /// <returns></returns>
    public IEnumerator LoadAsset(AssetReference assetReference, Transform root, Action<GameObject> successCallback, Action failCallback)
    {
        GameObject go;
        int hashCode = assetReference.GetHashCode();
        AsyncOperationHandle<GameObject> handle = assetReference.InstantiateAsync(root);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            go = handle.Result;
            resourceDic.Add(hashCode, go);
            successCallback.Invoke(go);
        }
        else
        {
            failCallback.Invoke();
        }
    }

    /// <summary>
    /// Asset을 Unload하는 함수
    /// </summary>
    /// <param name="assetReference">  Asset의 주소 </param>
    /// <param name="go"> Addressable로 로드 및 instantiate된 GameObejct</param>
    public void UnLoad(AssetReference assetReference, GameObject go)
    {
        resourceDic.Remove(assetReference.GetHashCode());
        assetReference.ReleaseInstance(go);
    }

    public void Init() { }
    public void UpdateFrame() { }
    public void UpdateSec() { }
    public void Clear() { }
}