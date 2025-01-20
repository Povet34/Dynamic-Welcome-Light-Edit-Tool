using UnityEngine;
using UnityEngine.UI;

public class YAxesLine : MonoBehaviour
{
    public class Info
    {
        public int time;
        public Vector2 size;
        public float posX;
        public float rayCastTargetXAxisPadding;
    }

    int time;
    float posX;
    public int Time => time;
    public float PosX => posX;

    public void Init(Info info)
    {
        time = info.time;
        posX = info.posX;

        var rt = GetComponent<RectTransform>();
        if (rt)
        {
            rt.sizeDelta = info.size;
            rt.anchoredPosition = new Vector2(posX, 0);
            rt.GetComponent<Image>().raycastPadding = 
                new Vector4(
                    -info.rayCastTargetXAxisPadding, 
                    0,
                    -info.rayCastTargetXAxisPadding,
                    0
                );
        }
    }
}
