using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// When the player's head gets very close to this shadow, plays a death sequence:
/// a 3D scream (the charging shadow makes it feel like it rushes the player), the
/// view fades to black, then the game returns to the main menu.
/// Put this on a shadow (IShadow) GameObject.
/// </summary>
public class ShadowDeathSequence : MonoBehaviour
{
    [Header("Trigger")]
    [Tooltip("Distance (m) from the player's head that triggers death.")]
    [SerializeField] private float triggerDistance = 0.8f;

    [Header("Scream")]
    [SerializeField] private AudioClip screamClip;
    [SerializeField, Range(0f, 1f)] private float screamVolume = 1f;

    [Header("Fade / scene")]
    [SerializeField] private float fadeDuration = 1.6f;
    [SerializeField] private float holdBlack = 0.6f;
    [SerializeField] private string menuSceneName = "UI_MainMenu_VR";

    private Transform head;
    private bool dead;

    private void Start()
    {
        if (Camera.main != null) head = Camera.main.transform;
    }

    private void Update()
    {
        if (dead) return;

        if (head == null)
        {
            if (Camera.main == null) return;
            head = Camera.main.transform;
        }

        // Horizontal distance only: the shadow's pivot is at the floor while the
        // player's head is ~1.6 m up, so a full 3D distance would never get small.
        Vector3 a = transform.position; a.y = 0f;
        Vector3 b = head.position; b.y = 0f;

        if (Vector3.Distance(a, b) <= triggerDistance)
            StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        dead = true;

        // 3D scream from the shadow (spatialised -> feels like it rushes the player).
        if (screamClip != null)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.clip = screamClip;
            src.volume = screamVolume;
            src.spatialBlend = 1f;
            src.dopplerLevel = 0f;
            src.Play();
        }

        // Full-screen black fade locked to the head.
        var canvasGO = new GameObject("DeathFade");
        canvasGO.transform.SetParent(head, false);
        canvasGO.transform.localPosition = new Vector3(0f, 0f, 0.3f);
        canvasGO.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        var rt = (RectTransform)canvasGO.transform;
        rt.sizeDelta = new Vector2(2f, 2f); // covers the whole FOV at 0.3 m

        var group = canvasGO.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        var imgGO = new GameObject("Black");
        imgGO.transform.SetParent(canvasGO.transform, false);
        var img = imgGO.AddComponent<Image>();
        img.color = Color.black;
        img.raycastTarget = false;
        var irt = img.rectTransform;
        irt.anchorMin = Vector2.zero;
        irt.anchorMax = Vector2.one;
        irt.offsetMin = Vector2.zero;
        irt.offsetMax = Vector2.zero;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        group.alpha = 1f;

        yield return new WaitForSeconds(holdBlack);

        SceneManager.LoadScene(menuSceneName);
    }
}
