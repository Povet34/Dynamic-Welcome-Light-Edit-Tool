using UnityEngine;

public class LoadingPopup : MonoBehaviour
{
    [SerializeField] Transform loadingTr;
    [SerializeField] float rotateSpeed;

    public void Show(bool show) => gameObject.SetActive(show);

    public void Update()
    {
        if (loadingTr)
            loadingTr.Rotate(Vector3.forward * -rotateSpeed * Time.deltaTime);
    }
}
