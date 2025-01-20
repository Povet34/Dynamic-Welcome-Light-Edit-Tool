using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMngr : MonoBehaviour, IMngr
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.eScene sceneType)
    {
        SceneManager.LoadScene(GetSceneName(sceneType));
    }

    public void ClearScene()
    {
        CurrentScene.Clear();
    }

    private string GetSceneName(Define.eScene sceneType)
    { 
        string name = System.Enum.GetName(typeof(Define.eScene), sceneType);
        return name;
    }

    public void Clear() { }
    public void Init() { }
    public void UpdateFrame() { }
    public void UpdateSec() { }
}