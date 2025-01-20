using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;

public partial class GenerateEditor : EditorWindow
{
    private GameObject uiPrefab = null;
    private bool isFullScreen = false;
    private bool isCaching =  false;
    private bool isIgnoreEscape = false;

    private void GenerateUI()
    {
        AddTip("[GenerateUI]");
        EditorGUILayout.BeginHorizontal();
        this.uiPrefab = EditorGUILayout.ObjectField("UI Prefab", uiPrefab, typeof(GameObject), true) as GameObject;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        this.isFullScreen = EditorGUILayout.Toggle("IsFullScreen", this.isFullScreen, GUILayout.Width(60f));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        this.isCaching = EditorGUILayout.Toggle("IsCaching", this.isCaching, GUILayout.Width(60f));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        this.isIgnoreEscape = EditorGUILayout.Toggle("IsIgnoreEscape", this.isIgnoreEscape, GUILayout.Width(60f));
        EditorGUILayout.EndHorizontal();

        if (null != this.uiPrefab)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate UI Code", GUILayout.Width(220)))
            {
                GenerateUICode();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Convert Custom UI", GUILayout.Width(220)))
            {
                ConvertCustomUI();
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Attach UI Component", GUILayout.Width(220)))
            {
                AttachUIComponent();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Register UI Addressable", GUILayout.Width(220)))
            {
                RegisterUIAddressableData();
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space();
    }

    #region Generate UI Code : ------------------------------------------------

    private void GenerateUICode()
    {
        string uiName = this.uiPrefab.name;

        string texts = GetObjectsString<UITextMeshPro>(this.uiPrefab);
        string images = GetObjectsString<UIImage>(this.uiPrefab);
        string buttons = GetObjectsString<UIButton>(this.uiPrefab);
        string toggles = GetObjectsString<UIToggle>(this.uiPrefab);
        string rawImages = GetObjectsString<UIRawImage>(this.uiPrefab);
        string inputFields = GetObjectsString<UIInputField>(this.uiPrefab);
        string sliders = GetObjectsString<UISlider>(this.uiPrefab);
        string gameObjs = GetObjectsString<RectTransform>(this.uiPrefab);

        (string method, string assign) buttonCodes = GetEventHandlerButtonCode(buttons);
        //(string method, string assign) siliderCodes = Get(sliders);
        (string method, string assign) toggleCodes = GetEventHandlerToggleCode(toggles);
        (string method, string assign) inputFieldCodes = GetEventHandlerInputFieldCode(inputFields);

        bool isFirstLine = true;
        StringBuilder sbEnums = new StringBuilder();
        StringBuilder sbGets = new StringBuilder();
        StringBuilder sbBind = new StringBuilder();
        if (!string.IsNullOrEmpty(texts))
        {
            string textEnum = GetObjectEnumString("Texts", texts);
            sbEnums.AppendLine(textEnum);
            string textGet = GetObjectGetString<UITextMeshPro>(texts);
            sbGets.AppendLine(textGet);

            if (isFirstLine)
            {
                sbBind.AppendLine("Bind<UITextMeshPro>(typeof(Texts));");
                isFirstLine = false;
            }
            else
            {
                sbBind.AppendLine("\t\t\tBind<UITextMeshPro>(typeof(Texts));");
            }
        }
        if (!string.IsNullOrEmpty(images)) 
        {
            string imageEnum = GetObjectEnumString("Images", images);
            sbEnums.AppendLine(imageEnum);
            string imageGet = GetObjectGetString<UIImage>(images);
            sbGets.AppendLine(imageGet);

            if (isFirstLine)
            {
                sbBind.AppendLine("Bind<UIImage>(typeof(Images));");
                isFirstLine = false;
            }
            else
            {
                sbBind.AppendLine("\t\t\tBind<UIImage>(typeof(Images));");
            }
        }
        if (!string.IsNullOrEmpty(rawImages))
        {
            string imageEnum = GetObjectEnumString("RawImages", rawImages);
            sbEnums.AppendLine(imageEnum);
            string imageGet = GetObjectGetString<UIRawImage>(rawImages);
            sbGets.AppendLine(imageGet);

            if (isFirstLine)
            {
                sbBind.AppendLine("Bind<UIRawImage>(typeof(RawImages));");
                isFirstLine = false;
            }
            else
            {
                sbBind.AppendLine("\t\t\tBind<UIRawImage>(typeof(RawImages));");
            }
        }
        if (!string.IsNullOrEmpty(buttons))
        {
            string buttonEnum = GetObjectEnumString("Buttons", buttons);
            sbEnums.AppendLine(buttonEnum);
            string buttonsGet = GetObjectGetString<UIButton>(buttons);
            sbGets.AppendLine(buttonsGet);

            if (isFirstLine)
            {
                sbBind.AppendLine("Bind<UIButton>(typeof(Buttons));");
                isFirstLine = false;
            }
            else
            {
                sbBind.AppendLine("\t\t\tBind<UIButton>(typeof(Buttons));");
            }
        }
        if (!string.IsNullOrEmpty(toggles))
        {
            string toggleEnum = GetObjectEnumString("Toggles", toggles);
            sbEnums.AppendLine(toggleEnum);
            string toggleGet = GetObjectGetString<UIToggle>(toggles);
            sbGets.AppendLine(toggleGet);

            if (isFirstLine)
            {
                sbBind.AppendLine("Bind<UIToggle>(typeof(Toggles));");
                isFirstLine = false;
            }
            else
            {
                sbBind.AppendLine("\t\t\tBind<UIToggle>(typeof(Toggles));");
            }
        }
        if (!string.IsNullOrEmpty(inputFields))
        {
            string inputFieldEnum = GetObjectEnumString("InputFields", inputFields);
            sbEnums.AppendLine(inputFieldEnum);
            string inputFieldGet = GetObjectGetString<UIInputField>(inputFields);
            sbGets.AppendLine(inputFieldGet);

            if (isFirstLine) 
            {
                sbBind.AppendLine("Bind<UIInputField>(typeof(InputFields));");
                isFirstLine = false;
            }
            else
            {
                sbBind.AppendLine("\t\t\tBind<UIInputField>(typeof(InputFields));");
            }
        }
        if (!string.IsNullOrEmpty(gameObjs))
        {
            string gameObjectEnum = GetObjectEnumString("GameObjects", gameObjs);
            sbEnums.AppendLine(gameObjectEnum);
            string gameObjectGet = GetObjectGetString<GameObject>(gameObjs);
            sbGets.AppendLine(gameObjectGet);

            if (isFirstLine) 
            {
                sbBind.Append("Bind<GameObject>(typeof(GameObjects));");
                isFirstLine = false;
            }
            else
            {
                sbBind.Append("\t\t\tBind<GameObject>(typeof(GameObjects));");
            }
        }

        StringBuilder sbMethod = new StringBuilder();
        if (!string.IsNullOrEmpty(buttonCodes.method))
        {
            sbMethod.Append(buttonCodes.method);
        }

        if (!string.IsNullOrEmpty(toggleCodes.method))
        {
            if (sbMethod.Length > 0)
            {
                sbMethod.Append("\n\n\t\t");
            }

            sbMethod.Append(toggleCodes.method);
        }

        if (!string.IsNullOrEmpty(inputFieldCodes.method))
        {
            if (sbMethod.Length > 0)
            {
                sbMethod.Append("\n\n\t\t");
            }

            sbMethod.Append(inputFieldCodes.method);
        }

        //if (!string.IsNullOrEmpty(sliders))
        //{
        //    if (sbMethod.Length > 0)
        //    {
        //        sbMethod.Append("\n\n\t\t");
        //    }

        //    sbMethod.Append(sliders.method);
        //}

        StringBuilder sbAssign = new StringBuilder();
        if (!string.IsNullOrEmpty(buttonCodes.assign)) 
        {
            sbAssign.Append(buttonCodes.assign);
        }

        if (!string.IsNullOrEmpty(toggleCodes.assign))
        {
            if (sbAssign.Length > 0) 
            {  
                sbAssign.Append("\n\n\t\t\t"); 
            }

            sbAssign.Append(toggleCodes.assign);
        }

        if (!string.IsNullOrEmpty(inputFieldCodes.assign))
        {
            if (sbAssign.Length > 0)
            {
                sbAssign.Append("\n\n\t\t\t");
            }

            sbAssign.Append(inputFieldCodes.assign);
        }

        string generateCode =
$@"using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

namespace Common.UI
{{
    public class {uiName} : UIBase
    {{
        #region Enum Object : -------------------------------------------------
        {sbEnums}
        #endregion

        #region Accessor : ----------------------------------------------------
{sbGets}
        #endregion

        protected override void Awake()
        {{
            base.Awake();
            {sbBind}
            RegisterEventHandler();
        }}

        public override void OnInit() {{}}

        public override void OnActive() 
        {{
            base.OnActive();
        }}
        
        public override void OnInactive() {{}}
        public override void OnUpdateFrame() {{}}
        public override void OnUpdateSec() {{}}
        public override void OnClear() {{}}
        public override bool IsEscape() {{ return base.IsEscape(); }}

        #region UI Event : ----------------------------------------------------

        #region Register Event Handler : ----------------------------
        private void RegisterEventHandler()
        {{
            {sbAssign}
        }}
        #endregion

        {sbMethod}

        #endregion
    }}
}}";
        string fullPath = string.Empty;
        CreateAndGetFullPath(ref fullPath);
        File.WriteAllText(fullPath, generateCode, Encoding.UTF8);
    }
    #endregion

    #region Attach UI Component : -----------------------------------
    private void AttachUIComponent()
    {
        if (null != this.uiPrefab)
        {
            uiPrefab.GetOrAddComponent<Canvas>();
            uiPrefab.GetOrAddComponent<GraphicRaycaster>();
        }
    }
    #endregion

    #region UI Addressable : ----------------------------------------
    private void RegisterUIAddressableData()
    {
        UIAddressable asset = (UIAddressable)AssetDatabase.LoadAssetAtPath("Assets/Resources/UIAddressable.asset", typeof(UIAddressable));
        if (null != asset)
        {
            string name = this.uiPrefab.name.Replace("UI", string.Empty);
            eUIType uiType = ConvertStringToUIType(name);
            var assetRef = new AssetReference();
            assetRef.SetEditorAsset(this.uiPrefab);
            asset.SetUIAddressableDataByUIType(uiType, this.isIgnoreEscape, this.isFullScreen, this.isCaching, assetRef);
        }
    }

    private eUIType ConvertStringToUIType(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            if (Enum.IsDefined(typeof(eUIType), str))
            {
                return (eUIType)Enum.Parse(typeof(eUIType), str);
            }
        }
        return eUIType.None;
    }
    #endregion

    #region Convert UI To CustomUI : --------------------------------
    private void ConvertCustomUI()
    {
        GameObject instance = Instantiate(uiPrefab);

        //AllRemoveUnnecessary(instance);
        ReplaceTextToUIText(instance);
        ReplaceComponent<UnityEngine.UI.Image, UIImage>(instance);
        ReplaceComponent<RawImage, UIRawImage>(instance);

        PrefabUtility.SaveAsPrefabAsset(instance, AssetDatabase.GetAssetPath(uiPrefab));
        DestroyImmediate(instance);
    }

    //private void AllRemoveUnnecessary(GameObject gameObject)
    //{
    //    var removes = gameObject.GetComponentsInChildren<FCU_Meta>();
    //    foreach (var remove in removes) 
    //        DestroyImmediate(remove);
    //}

    private void ReplaceComponent<T1, T2>(GameObject gameObject) where T1 : MonoBehaviour where T2 : MonoBehaviour
    {
        var textComponents = gameObject.GetComponentsInChildren<T1>();

        foreach (var textComponent in textComponents)
        {
            var tmpGO = new GameObject("tempOBJ");
            var inst = tmpGO.AddComponent<T2>();
            MonoScript yourReplacementScript = MonoScript.FromMonoBehaviour(inst);
            DestroyImmediate(tmpGO);

            SerializedObject so = new SerializedObject(textComponent);
            SerializedProperty scriptProperty = so.FindProperty("m_Script");
            so.Update();
            scriptProperty.objectReferenceValue = yourReplacementScript;
            so.ApplyModifiedProperties();
        }
    }

    private void ReplaceTextToUIText(GameObject gameObject)
    {
        var texts = gameObject.GetComponentsInChildren<Text>();
        foreach (var originalText in texts)
        {
            GameObject gameObj = originalText.gameObject;

            string txt = originalText.text;
            Color color = originalText.color;
            int fontSize = originalText.fontSize;
            FontStyles fontStyles = (FontStyles)originalText.fontStyle;
            TextAlignmentOptions textAlignment = ConvertHorizontalAlignment(originalText.alignment);

            DestroyImmediate(originalText);

            UITextMeshPro newText = gameObj.AddComponent<UITextMeshPro>();
            newText.text = txt;
            newText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Pretendard-Medium SDF");
            newText.fontSize = fontSize;
            newText.enableAutoSizing = true;
            newText.fontSizeMax = fontSize;
            newText.fontSizeMin = (fontSize - 10) > 0 ? fontSize - 10 : 0;
            newText.fontStyle = fontStyles;
            newText.alignment = textAlignment;
            newText.color = color;
            // Temp
            if (newText.fontStyle.HasFlag(FontStyles.Bold))
                newText.fontStyle &= ~FontStyles.Bold;
        }
    }

    private TextAlignmentOptions ConvertHorizontalAlignment(TextAnchor alignment)
    {
        switch (alignment)
        {
            case TextAnchor.UpperLeft:
                return TextAlignmentOptions.TopLeft;
            case TextAnchor.UpperCenter:
                return TextAlignmentOptions.Top;
            case TextAnchor.UpperRight:
                return TextAlignmentOptions.TopRight;
            case TextAnchor.MiddleLeft:
                return TextAlignmentOptions.Left;
            case TextAnchor.MiddleCenter:
                return TextAlignmentOptions.Center;
            case TextAnchor.MiddleRight:
                return TextAlignmentOptions.Right;
            case TextAnchor.LowerLeft:
                return TextAlignmentOptions.BottomLeft;
            case TextAnchor.LowerCenter:
                return TextAlignmentOptions.Bottom;
            case TextAnchor.LowerRight:
                return TextAlignmentOptions.BottomRight;
            default:
                return TextAlignmentOptions.Center;
        }
    }
    #endregion

    private void CreateAndGetFullPath(ref string fullPath)
    {
        if (!Directory.Exists(Define.GENERATE_UI_FOLDER_PATH))
            Directory.CreateDirectory(Define.GENERATE_UI_FOLDER_PATH);

        fullPath = Define.GENERATE_UI_FOLDER_PATH + this.uiPrefab.name + ".cs";
    }

    private string GetObjectsString<T>(GameObject uiPrefab) where T : UnityEngine.Object
    {
        StringBuilder sbEnum = new StringBuilder();
        if (null != uiPrefab)
        {
            T[] arr = uiPrefab.GetComponentsInChildren<T>();
            if (null != arr && arr.Length > 0)
            {
                int sameIndex = 0;
                for (int index = 0; index < arr.Length; index++)
                {
                    if (IsValidObject<T>(arr[index].name))
                    {
                        if (sameIndex != 0)
                            sbEnum.Append("\n\t\t\t");

                        sbEnum.Append(arr[index].name);
                        sbEnum.Append(",");
                        sameIndex++;
                    }
                }
            }
        }

       return sbEnum.ToString();
    }

    private string GetObjectEnumString(string enumName, string enums)
    {
        if (!string.IsNullOrEmpty(enumName) && !string.IsNullOrEmpty(enums)) 
        {
            string enumCode = $@"
        private enum {enumName}
        {{
            {enums}
        }}";
            return enumCode;
        }
        
        return string.Empty;
    }

    private string GetObjectGetString<T>(string enums) where T : UnityEngine.Object
    {
        StringBuilder sb = new StringBuilder();
        if (!string.IsNullOrEmpty(enums))
        {
            enums = enums.Replace("\n", string.Empty);
            enums = enums.Replace("\t", string.Empty);
            if (typeof(T) == typeof(UITextMeshPro))
            {
                var strArr = enums.Split(',');
                foreach (var str in strArr)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        sb.Append("\t\t");
                        sb.Append($@"private UITextMeshPro {char.ToLower(str[0]) + str.Substring(1)} => GetText(Convert.ToInt32(Texts.{str}));");
                        sb.Append("\n");
                    }
                }
            }
            else if (typeof(T) == typeof(UIImage))
            {
                var strArr = enums.Split(',');
                foreach (var str in strArr)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        sb.Append("\t\t");
                        sb.Append($@"private UIImage {char.ToLower(str[0]) + str.Substring(1)} => GetImage(Convert.ToInt32(Images.{str}));");
                        sb.Append("\n");
                    }
                }
            }
            else if (typeof(T) == typeof(UIRawImage))
            {
                var strArr = enums.Split(',');
                foreach (var str in strArr)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        sb.Append("\t\t");
                        sb.Append($@"private UIRawImage {char.ToLower(str[0]) + str.Substring(1)} => GetRawImage(Convert.ToInt32(RawImages.{str}));");
                        sb.Append("\n");
                    }
                }
            }
            else if (typeof(T) == typeof(UIButton))
            {
                var strArr = enums.Split(',');
                foreach (var str in strArr)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        sb.Append("\t\t");
                        sb.Append($@"private UIButton {char.ToLower(str[0]) + str.Substring(1)} => GetButton(Convert.ToInt32(Buttons.{str}));");
                        sb.Append("\n");
                    }
                }
            }
            else if (typeof(T) == typeof(UIToggle))
            {
                var strArr = enums.Split(',');
                foreach (var str in strArr)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        sb.Append("\t\t");
                        sb.Append($@"private UIToggle {char.ToLower(str[0]) + str.Substring(1)} => GetToggle(Convert.ToInt32(Toggles.{str}));");
                        sb.Append("\n");
                    }
                }
            }
            else if (typeof(T) == typeof(UIInputField))
            {
                var strArr = enums.Split(',');
                foreach (var str in strArr)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        sb.Append("\t\t");
                        sb.Append($@"private UIInputField {char.ToLower(str[0]) + str.Substring(1)} => GetInputField(Convert.ToInt32(InputFields.{str}));");
                        sb.Append("\n");
                    }
                }
            }
            else
            {
                var strArr = enums.Split(',');
                foreach (var str in strArr)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        sb.Append("\t\t");
                        sb.Append($@"private GameObject {char.ToLower(str[0]) + str.Substring(1)} => GetGameObject(Convert.ToInt32(GameObjects.{str}));");
                        sb.Append("\n");
                    }
                }
            }
        }

        return sb.ToString();
    }

    private bool IsValidObject<T>(string objName) where T : UnityEngine.Object
    {
        if (typeof(T) == typeof(UITextMeshPro) && objName.Contains("Text"))
            return true;
        else if (typeof(T) == typeof(UIImage) && objName.Contains("Image"))
            return true;
        else if (typeof(T) == typeof(UIRawImage) && objName.Contains("RawImage"))
            return true;
        else if (typeof(T) == typeof(UIButton) && objName.Contains("Button"))
            return true;
        else if (typeof(T) == typeof(UIToggle) && objName.Contains("Toggle"))
            return true;
        else if (typeof(T) == typeof(UIInputField) && objName.Contains("InputField"))
            return true;
        else if (typeof(T) == typeof(RectTransform) && objName.Contains("Go"))
            return true;

        return false;
    }

    #region Event Handler Method : --------------------------------------------
    private (string, string) GetEventHandlerButtonCode(string buttons)
    {
        StringBuilder sbMethod = new StringBuilder();
        StringBuilder sbAssign = new StringBuilder();
        if (!string.IsNullOrEmpty(buttons))
        {
            char[] delimiter = { ',', '\t', '\n' };
            string[] strArr = buttons.Split(delimiter, System.StringSplitOptions.RemoveEmptyEntries);

            for (int index = 0; index < strArr.Length; index++)
            {
                string methodCode =
$@"public void On{strArr[index]}(string buttonData)
        {{

        }}";
                sbMethod.Append(methodCode);

                if (index + 1 < strArr.Length)
                    sbMethod.Append("\n\n\t\t");

                string assignCode =
$@"{char.ToLower(strArr[index][0]) + strArr[index].Substring(1)}.BindButtonEvent(string.Empty, On{strArr[index]});";
                sbAssign.Append(assignCode);

                if (index + 1 < strArr.Length)
                    sbAssign.Append("\n\t\t\t");
            }
        }

        return (sbMethod.ToString(), sbAssign.ToString());
    }

    private (string, string) GetEventHandlerToggleCode(string toggles)
    {
        StringBuilder sbMethod = new StringBuilder();
        StringBuilder sbAssign = new StringBuilder();
        if (!string.IsNullOrEmpty(toggles))
        {
            char[] delimiter = { ',', '\t', '\n' };
            string[] strArr = toggles.Split(delimiter, System.StringSplitOptions.RemoveEmptyEntries);

            for (int index = 0; index < strArr.Length; index++)
            {
                string methodCode =
$@"public void On{strArr[index]}(bool isOn, string toggleData)
        {{

        }}";
                sbMethod.Append(methodCode);

                if (index + 1 < strArr.Length)
                    sbMethod.Append("\n\n\t\t");

                string assignCode =
$@"{char.ToLower(strArr[index][0]) + strArr[index].Substring(1)}.BindToggleEvent(string.Empty, On{strArr[index]});";
                sbAssign.Append(assignCode);

                if (index + 1 < strArr.Length)
                    sbAssign.Append("\n\t\t\t");
            }

        }

        return (sbMethod.ToString(), sbAssign.ToString());
    }

    private (string, string) GetEventHandlerInputFieldCode(string inputFields)
    {
        StringBuilder sbMethod = new StringBuilder();
        StringBuilder sbAssign = new StringBuilder();
        if (!string.IsNullOrEmpty(inputFields))
        {
            char[] delimiter = { ',', '\t', '\n' };
            string[] strArr = inputFields.Split(delimiter, System.StringSplitOptions.RemoveEmptyEntries);

            for (int index = 0; index < strArr.Length; index++) 
            {
                string methodCode =
$@"public void On{strArr[index]}EndEdit(string text)
        {{

        }}

        public void On{strArr[index]}Submit(string text)
        {{

        }}

        public void On{strArr[index]}ValueChanged(string text)
        {{

        }}";
                sbMethod.Append(methodCode);

                if (index + 1 < strArr.Length)
                    sbMethod.Append("\n\n\t\t");

                string assignCode =
$@"{char.ToLower(strArr[index][0]) + strArr[index].Substring(1)}.BindInputFieldEvent(On{strArr[index]}EndEdit, On{strArr[index]}Submit, On{strArr[index]}ValueChanged);";
                sbAssign.Append(assignCode);

                if (index + 1 < strArr.Length)
                    sbAssign.Append("\n\t\t\t");
            }
        }

        return (sbMethod.ToString(), sbAssign.ToString());
    }
    #endregion
}