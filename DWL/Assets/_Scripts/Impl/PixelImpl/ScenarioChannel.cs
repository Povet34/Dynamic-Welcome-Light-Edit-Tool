using UnityEngine;
namespace Common.UI
{
    public class ScenarioChannel : UIBase
    {
        [SerializeField] UIImage bg;
        [SerializeField] UITextMeshPro indexText;

        RectTransform rectTransform;
        UIImage img;

        public void Init(Vector2 pos, int index, bool isShowIndex)
        {
            rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = pos;

            img = GetComponent<UIImage>();

            indexText.text = index.ToString("D2");
            ShowIndex(isShowIndex);
        }

        public void SetColor(Color32 color)
        {
            if (img)
                img.color = color;
        }

        public void ShowIndex(bool isShow)
        {
            bg?.gameObject.SetActive(isShow);
        }
    }
}