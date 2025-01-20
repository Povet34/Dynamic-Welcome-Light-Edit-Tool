using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomUIMenuOptions
{
    private const string kUILayerName = "UI";

    #region Custom Object : ---------------------------------------------------
    [MenuItem("GameObject/UI/Custom/UIText", false, 0)]
    public static void CreateCustomTextMeshPro(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        if (null == parent || null == parent.GetComponentInParent<Canvas>())
            parent = GetOrCreateCanvasGameObject();

        var gameObj = new GameObject("UIText");
        gameObj.transform.SetParent(parent.transform, false);
        gameObj.transform.localScale = Vector3.one;
        gameObj.transform.localPosition = Vector3.zero;
        gameObj.AddComponent<UITextMeshPro>();

        int layer = LayerMask.NameToLayer("UI");
        ChangeLayerRecursively(gameObj, layer);
    }

    [MenuItem("GameObject/UI/Custom/UIImage", false, 1)]
    public static void CreateCustomImage(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        if (null == parent || null == parent.GetComponentInParent<Canvas>())
            parent = GetOrCreateCanvasGameObject();

        var gameObj = new GameObject("UIImage");
        gameObj.transform.SetParent(parent.transform, false);
        gameObj.transform.localScale = Vector3.one;
        gameObj.transform.localPosition = Vector3.zero;
        gameObj.AddComponent<UIImage>();

        int layer = LayerMask.NameToLayer("UI");
        ChangeLayerRecursively(gameObj, layer);
    }

    [MenuItem("GameObject/UI/Custom/UIButton", false, 2)]
    public static void CreateCustomButton(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        if (null == parent || null == parent.GetComponentInParent<Canvas>())
            parent = GetOrCreateCanvasGameObject();

        var gameObj = new GameObject("UIButton");
        gameObj.transform.SetParent(parent.transform, true);
        gameObj.transform.localScale = Vector3.one;
        gameObj.transform.localPosition = Vector3.zero;

        UIImage customImage = gameObj.AddComponent<UIImage>();
        UIButton customButton = gameObj.AddComponent<UIButton>();
        if (null != customImage && null != customButton)
            customButton.targetGraphic = customImage;

        int layer = LayerMask.NameToLayer("UI");
        ChangeLayerRecursively(gameObj, layer);
    }

    [MenuItem("GameObject/UI/Custom/UIToggle", false, 3)]
    public static void CreateCustomToggle(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        if (null == parent || null == parent.GetComponentInParent<Canvas>())
            parent = GetOrCreateCanvasGameObject();

        var gameObj = new GameObject("UIToggle");
        gameObj.transform.SetParent(parent.transform, true);
        gameObj.transform.localScale = Vector3.one;
        gameObj.transform.localPosition = Vector3.zero;

        UIImage customImage = gameObj.AddComponent<UIImage>();
        UIToggle customToggle = gameObj.AddComponent<UIToggle>();
        if (null != customImage && null != customToggle)
            customToggle.targetGraphic = customImage;

        var onObj = new GameObject("OnState", typeof(RectTransform));
        onObj.transform.SetParent(gameObj.transform, true);
        onObj.transform.localScale = Vector3.one;
        onObj.transform.localPosition = Vector3.zero;
        customToggle.GoOnState = onObj;

        var offObj = new GameObject("OffState", typeof(RectTransform));
        offObj.transform.SetParent(gameObj.transform, true);
        offObj.transform.localScale = Vector3.one;
        offObj.transform.localPosition = Vector3.zero;
        customToggle.GoOffState = offObj;

        int layer = LayerMask.NameToLayer("UI");
        ChangeLayerRecursively(gameObj, layer);
    }

    [MenuItem("GameObject/UI/Custom/UIInputField", false, 4)]
    public static void CreateCustomInputField(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        if (null == parent || null == parent.GetComponentInParent<Canvas>())
            parent = GetOrCreateCanvasGameObject();

        var gameObj = new GameObject("UIInputField", typeof(RectTransform));
        gameObj.transform.SetParent(parent.transform, true);
        gameObj.transform.localScale = Vector3.one;
        gameObj.transform.localPosition = Vector3.zero;
        RectTransform rt = gameObj.transform as RectTransform;
        rt.sizeDelta = new Vector2(160, 30);

        UIImage customImage = gameObj.AddComponent<UIImage>();
        UIInputField customInputField = gameObj.AddComponent<UIInputField>();
        if (null != customImage && null != customInputField)
        {
            customImage.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
            customImage.type = Image.Type.Sliced;
            customInputField.targetGraphic = customImage;

            var textArea = new GameObject("Text Area", typeof(RectTransform), typeof(RectMask2D));
            if (null != textArea)
            {
                textArea.transform.SetParent(gameObj.transform, false);
                textArea.transform.localScale = Vector3.one;
                textArea.transform.localPosition = Vector3.zero;
                RectTransform rtTextArea = textArea.transform as RectTransform;
                if (null != rtTextArea)
                {
                    rtTextArea.anchorMin = new Vector2(0, 0);
                    rtTextArea.anchorMax = new Vector2(1, 1);
                    rtTextArea.offsetMin = new Vector2(10, 7);
                    rtTextArea.offsetMax = new Vector2(-10, -6);

                    customInputField.textViewport = rtTextArea;
                }

                RectMask2D mask = textArea.GetComponent<RectMask2D>();
                if (null != mask)
                    mask.padding = new Vector4(-8, -5, -8, -5);

                var placeholder = new GameObject("placeHolder", typeof(RectTransform), typeof(CanvasRenderer), typeof(UITextMeshPro), typeof(LayoutElement));
                if (null != placeholder)
                {
                    placeholder.transform.SetParent(textArea.transform, true);
                    placeholder.transform.localScale = Vector3.one;
                    placeholder.transform.localPosition = Vector3.zero;
                    RectTransform rtPlaceholder = placeholder.transform as RectTransform;
                    if (null != rtPlaceholder)
                    {
                        rtPlaceholder.anchorMin = new Vector2(0, 0);
                        rtPlaceholder.anchorMax = new Vector2(1, 1);
                        rtPlaceholder.offsetMin = new Vector2(0, 0);
                        rtPlaceholder.offsetMax = new Vector2(0, 0);
                    }

                    UITextMeshPro placeholderTextMeshPro = placeholder.GetComponent<UITextMeshPro>();
                    if (null != placeholderTextMeshPro)
                    {
                        placeholderTextMeshPro.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Pretendard-Medium SDF");
                        placeholderTextMeshPro.fontSize = 20;
                        placeholderTextMeshPro.enableAutoSizing = true;
                        placeholderTextMeshPro.fontSizeMax = 30;
                        placeholderTextMeshPro.fontSizeMin = 20;
                        placeholderTextMeshPro.enableWordWrapping = false;

                        Color color;
                        ColorUtility.TryParseHtmlString("#32323280", out color);
                        placeholderTextMeshPro.color = color;

                        customInputField.placeholder = placeholderTextMeshPro;
                    }

                    LayoutElement layoutElement = placeholder.GetComponent<LayoutElement>();
                    if (null != layoutElement)
                    {
                        layoutElement.ignoreLayout = true;
                        layoutElement.layoutPriority = 1;
                    }
                }

                var text = new GameObject("text", typeof(CanvasRenderer), typeof(UITextMeshPro));
                if (null != text)
                {
                    text.transform.SetParent(textArea.transform, true);
                    text.transform.localScale = Vector3.one;
                    text.transform.localPosition = Vector3.zero;
                    RectTransform rtText = text.transform as RectTransform;
                    if (null != rtText)
                    {
                        rtText.anchorMin = new Vector2(0, 0);
                        rtText.anchorMax = new Vector2(1, 1);
                        rtText.offsetMin = new Vector2(0, 0);
                        rtText.offsetMax = new Vector2(0, 0);
                    }

                    UITextMeshPro textMeshPro = text.GetComponent<UITextMeshPro>();
                    if (null != textMeshPro)
                    {
                        textMeshPro.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Pretendard-Medium SDF");
                        textMeshPro.fontSize = 20;
                        textMeshPro.enableAutoSizing = true;
                        textMeshPro.fontSizeMax = 30;
                        textMeshPro.fontSizeMin = 20;
                        textMeshPro.enableWordWrapping = false;

                        Color color;
                        ColorUtility.TryParseHtmlString("#323232ff", out color);
                        textMeshPro.color = color;

                        customInputField.textComponent = textMeshPro;
                    }
                }
            }
        }

        int layer = LayerMask.NameToLayer("UI");
        ChangeLayerRecursively(gameObj, layer);
    }

    [MenuItem("GameObject/UI/Custom/UISlider", false, 5)]
    public static void CreateCustomSlider(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        if (null == parent || null == parent.GetComponentInParent<Canvas>())
            parent = GetOrCreateCanvasGameObject();

        var gameObj = new GameObject("UISlider", typeof(RectTransform));
        gameObj.transform.SetParent(parent.transform, true);
        gameObj.transform.localScale = Vector3.one;
        gameObj.transform.localPosition = Vector3.zero;
        RectTransform rt = gameObj.transform as RectTransform;
        rt.sizeDelta = new Vector2(160, 20);

        //CustomImage customImage = gameObj.AddComponent<CustomImage>();
        UISlider customSlider = gameObj.AddComponent<UISlider>();
        if (null != customSlider)
        {
            var background = new GameObject("Background", typeof(RectTransform));
            background.transform.SetParent(gameObj.transform, true);
            background.transform.localScale = Vector3.one;
            background.transform.localPosition = Vector3.zero;
            RectTransform rtBackground = background.transform as RectTransform;
            if (null != rtBackground)
            {
                rtBackground.anchorMin = new Vector2(0, 0.25f);
                rtBackground.anchorMax = new Vector2(1, 0.75f);
                rtBackground.offsetMin = new Vector2(0, 0);
                rtBackground.offsetMax = new Vector2(0, 0);
            }
            UIImage backgroundImg = background.AddComponent<UIImage>();
            if (null != backgroundImg)
            {
                backgroundImg.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
                backgroundImg.type = Image.Type.Sliced;
            }

            var fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(gameObj.transform, true);
            fillArea.transform.localScale = Vector3.one;
            fillArea.transform.localPosition = Vector3.zero;
            RectTransform rtFillArea = fillArea.transform as RectTransform;
            if (null != rtFillArea)
            {
                rtFillArea.anchorMin = new Vector2(0, 0.25f);
                rtFillArea.anchorMax = new Vector2(1, 0.75f);
                rtFillArea.offsetMin = new Vector2(5, 0);
                rtFillArea.offsetMax = new Vector2(-15, 0);
            }

            var fill = new GameObject("Fill", typeof(RectTransform));
            fill.transform.SetParent(fillArea.transform, true);
            fill.transform.localScale = Vector3.one;
            fill.transform.localPosition = Vector3.zero;
            RectTransform rtFill = fill.transform as RectTransform;
            if (null != rtFill)
            {
                DrivenRectTransformTracker dt = new DrivenRectTransformTracker();
                dt.Clear();

                Object driver = gameObj;
                dt.Add(driver, rtFill, DrivenTransformProperties.Anchors);

                rtFill.anchorMin = new Vector2(0, 0);
                rtFill.anchorMax = new Vector2(0, 1);
                rtFill.offsetMin = new Vector2(0, 0);
                rtFill.offsetMax = new Vector2(0, 0);
                rtFill.sizeDelta = new Vector2(10, 0);

                customSlider.fillRect = rtFill;
            }

            UIImage fillImg = fill.AddComponent<UIImage>();
            if (null != fillImg)
            {
                fillImg.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                fillImg.type = Image.Type.Sliced;
            }

            var handleSlideArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleSlideArea.transform.SetParent(gameObj.transform, true);
            handleSlideArea.transform.localScale = Vector3.one;
            handleSlideArea.transform.localPosition = Vector3.zero;
            RectTransform rthandleSlideArea = handleSlideArea.transform as RectTransform;
            if (null != rthandleSlideArea)
            {
                rthandleSlideArea.anchorMin = new Vector2(0, 0);
                rthandleSlideArea.anchorMax = new Vector2(1, 1);
                rthandleSlideArea.offsetMin = new Vector2(10, 0);
                rthandleSlideArea.offsetMax = new Vector2(-10, 0);
            }

            var handle = new GameObject("Handle", typeof(RectTransform));
            handle.transform.SetParent(handleSlideArea.transform, true);
            handle.transform.localScale = Vector3.one;
            handle.transform.localPosition = Vector3.zero;
            RectTransform rtHandle = handle.transform as RectTransform;
            if (null != rtHandle)
            {
                DrivenRectTransformTracker dt = new DrivenRectTransformTracker();
                dt.Clear();

                Object driver = gameObj;
                dt.Add(driver, rtHandle, DrivenTransformProperties.Anchors);

                rtHandle.anchorMin = new Vector2(0, 0);
                rtHandle.anchorMax = new Vector2(0, 1);
                rtHandle.offsetMin = new Vector2(0, 0);
                rtHandle.offsetMax = new Vector2(0, 0);
                rtHandle.sizeDelta = new Vector2(20, 0);

                customSlider.handleRect = rtHandle;
            }

            UIImage handleImg = handle.AddComponent<UIImage>();
            if (null != handleImg)
            {
                handleImg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
                customSlider.targetGraphic = handleImg;
            }

            int layer = LayerMask.NameToLayer("UI");
            ChangeLayerRecursively(gameObj, layer);
        }
    }
    #endregion

    #region Layer : -----------------------------------------------------------
    private static void ChangeLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (var child in obj.transform)
            ChangeLayerRecursively((GameObject)child, layer);
    }
    #endregion

    // Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
    public static GameObject GetOrCreateCanvasGameObject()
    {
        GameObject selectedGo = Selection.activeGameObject;

        // Try to find a gameobject that is the selected GO or one if its parents.
        Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
        if (canvas != null && canvas.gameObject.activeInHierarchy)
            return canvas.gameObject;

        // No canvas in selection or its parents? Then use just any canvas..
        canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
        if (canvas != null && canvas.gameObject.activeInHierarchy)
            return canvas.gameObject;

        // No canvas in the scene at all? Then create a new one.
        return CreateNewUI();
    }

    public static GameObject CreateNewUI()
    {
        // Root for the UI
        var root = new GameObject("Canvas");
        root.layer = LayerMask.NameToLayer(kUILayerName);
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        root.AddComponent<CanvasScaler>();
        root.AddComponent<GraphicRaycaster>();
        Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

        // if there is no event system add one...
        CreateEventSystem(false);
        return root;
    }

    public static void CreateEventSystem(MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        CreateEventSystem(true, parent);
    }

    private static void CreateEventSystem(bool select)
    {
        CreateEventSystem(select, null);
    }

    private static void CreateEventSystem(bool select, GameObject parent)
    {
        var esys = Object.FindObjectOfType<EventSystem>();
        if (esys == null)
        {
            var eventSystem = new GameObject("EventSystem");
            GameObjectUtility.SetParentAndAlign(eventSystem, parent);
            esys = eventSystem.AddComponent<EventSystem>();
#if NEW_INPUT_SYSTEM
                eventSystem.AddComponent<InputSystemUIInputModule>();
#else
            eventSystem.AddComponent<StandaloneInputModule>();
#endif

            Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
        }

        if (select && esys != null)
            Selection.activeGameObject = esys.gameObject;
    }
}