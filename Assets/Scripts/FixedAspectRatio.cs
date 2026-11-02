using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FixedAspectRatio : MonoBehaviour
{
    [Header("Target Rasio (16:9)")]
    public float targetWidth = 1920f;
    public float targetHeight = 1080f;
    public Color barColor = Color.black;

    private Camera cam;
    private int lastScreenWidth = 0;
    private int lastScreenHeight = 0;

    void Awake()
    {
        cam = GetComponent<Camera>();
        CreateBackgroundCamera();
        ApplyAspectRatio();
    }

    void Update()
    {
        // Perbarui hanya jika resolusi atau orientasi layar berubah
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            ApplyAspectRatio();
        }
    }

    void ApplyAspectRatio()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        float targetAspect = targetWidth / targetHeight;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // Letterbox (bar hitam di atas & bawah jika layar terlalu kotak)
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
        else
        {
            // Pillarbox (bar hitam di kiri & kanan untuk layar HP panjang seperti 20:9)
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        }
    }

    void CreateBackgroundCamera()
    {
        // Hindari membuat duplikat jika sudah ada
        if (transform.Find("BlackBarCamera") != null) return;

        // Membuat kamera latar belakang agar area di luar 16:9 bersih berwarna hitam (tidak glitch/flicker)
        GameObject bgCamObj = new GameObject("BlackBarCamera");
        bgCamObj.transform.SetParent(this.transform);
        Camera bgCam = bgCamObj.AddComponent<Camera>();
        bgCam.depth = cam.depth - 10;
        bgCam.clearFlags = CameraClearFlags.SolidColor;
        bgCam.backgroundColor = barColor;
        bgCam.cullingMask = 0;
        bgCam.useOcclusionCulling = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureOnMainCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null && mainCam.GetComponent<FixedAspectRatio>() == null)
        {
            mainCam.gameObject.AddComponent<FixedAspectRatio>();
        }
    }
}
