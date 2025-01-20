using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum DeveoplomentEnvironmnet
{
    Dev,
    Stage,
    Live
}

public class OptionSettings : ScriptableObject
{
    [SerializeField]
    public eLanguage language = eLanguage.ko_KR;

    [SerializeField]
    public DeveoplomentEnvironmnet env = DeveoplomentEnvironmnet.Dev;

    [SerializeField]
    public string versionName = "1.0.0";

    [SerializeField]
    public int versionCode = 1;

    public const string NAME = "OptionSettings";

    private static OptionSettings instance = null;

    public static OptionSettings GetInstance()
    {
        if (instance == null)
        {
            instance = Resources.Load<OptionSettings>(NAME);
        }
        return instance;
    }
}