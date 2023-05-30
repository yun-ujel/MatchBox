using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class PixelPerfectUI : MonoBehaviour
{
    [SerializeField] private Camera canvasRenderCamera;
    private CanvasScaler canvasScaler;
    private PixelPerfectCamera pixelPerfectCamera;

    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        pixelPerfectCamera = Camera.main.GetComponent<PixelPerfectCamera>();

        if (canvasRenderCamera == null)
        {
            canvasRenderCamera = GetComponent<Canvas>().worldCamera;
        }

        canvasRenderCamera.gameObject.SetActive(false);
        canvasRenderCamera.gameObject.SetActive(true);

        canvasScaler.scaleFactor = pixelPerfectCamera.pixelRatio;
    }

    void LateUpdate()
    {
        canvasScaler.scaleFactor = pixelPerfectCamera.pixelRatio;
    }
}
