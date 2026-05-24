using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Cybersickness comfort vignette (tunnelling). Builds a lightweight black
/// radial overlay locked to the camera (no post-processing, so it doesn't hurt
/// framerate) and fades it in while the player moves — simulating a reduced FOV.
/// Faster movement = stronger vignette; standing still = none.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class VRComfortVignette : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Head camera the overlay locks to. Auto-found (CenterEyeAnchor / Camera.main) if empty.")]
    [SerializeField] private Transform cameraTransform;

    [Header("Strength")]
    [Tooltip("Max darkness of the periphery at full speed (0-1).")]
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 0.85f;
    [Tooltip("Speed (m/s) at which the vignette is fullest.")]
    [SerializeField] private float speedForMax = 4f;
    [Tooltip("Below this speed, no vignette.")]
    [SerializeField] private float speedDeadzone = 0.3f;
    [Tooltip("Ease in/out speed (alpha units per second).")]
    [SerializeField] private float fadeSpeed = 4f;

    [Header("Shape")]
    [SerializeField] private float distance = 0.5f;
    [Tooltip("Fraction of the radius that stays clear in the center.")]
    [SerializeField, Range(0f, 1f)] private float innerRadius = 0.45f;
    [Tooltip("Fraction of the radius where it becomes fully dark.")]
    [SerializeField, Range(0f, 1.5f)] private float outerRadius = 0.95f;

    private CharacterController cc;
    private CanvasGroup group;
    private float current;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();

        if (cameraTransform == null)
            cameraTransform = Camera.main != null ? Camera.main.transform : transform;

        // World-space canvas locked just in front of the head, facing the camera.
        var canvasGO = new GameObject("ComfortVignette");
        canvasGO.transform.SetParent(cameraTransform, false);
        canvasGO.transform.localPosition = new Vector3(0f, 0f, distance);
        canvasGO.transform.localRotation = Quaternion.Euler(0f, 180f, 0f); // face the camera
        canvasGO.transform.localScale = Vector3.one;

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        var rt = (RectTransform)canvasGO.transform;
        float size = distance * 2.4f; // covers a bit beyond the FOV
        rt.sizeDelta = new Vector2(size, size);

        group = canvasGO.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        var imgGO = new GameObject("Vignette");
        imgGO.transform.SetParent(canvasGO.transform, false);
        var img = imgGO.AddComponent<RawImage>();
        img.raycastTarget = false;
        img.texture = BuildRadialTexture();
        img.color = Color.white; // texture already carries black + alpha
        var irt = img.rectTransform;
        irt.anchorMin = Vector2.zero;
        irt.anchorMax = Vector2.one;
        irt.offsetMin = Vector2.zero;
        irt.offsetMax = Vector2.zero;
    }

    private Texture2D BuildRadialTexture()
    {
        const int s = 256;
        var tex = new Texture2D(s, s, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
        var pixels = new Color32[s * s];
        Vector2 c = new Vector2(0.5f, 0.5f);

        for (int y = 0; y < s; y++)
        {
            for (int x = 0; x < s; x++)
            {
                float d = Vector2.Distance(new Vector2(x / (float)(s - 1), y / (float)(s - 1)), c) / 0.5f;
                float a = Mathf.Clamp01((d - innerRadius) / Mathf.Max(0.01f, outerRadius - innerRadius));
                pixels[y * s + x] = new Color32(0, 0, 0, (byte)(a * 255f));
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();
        return tex;
    }

    private void Update()
    {
        if (group == null) return;

        Vector3 v = cc.velocity;
        v.y = 0f;
        float t = Mathf.Clamp01((v.magnitude - speedDeadzone) / Mathf.Max(0.01f, speedForMax - speedDeadzone));
        float target = t * maxAlpha;

        current = Mathf.MoveTowards(current, target, fadeSpeed * Time.deltaTime);
        group.alpha = current;
    }
}
