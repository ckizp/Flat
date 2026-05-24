using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameTitle;
    [SerializeField] private CanvasGroup menuCanvasGroup;
    [SerializeField] private Light moodLight;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject mainButtonsRoot;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    private Vector3 initialRigPos;
    private GameObject rig;

    private void Start()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainButtonsRoot != null) mainButtonsRoot.SetActive(true);
rig = GameObject.Find("XR Origin (XR Rig)");
        if (rig != null) initialRigPos = rig.transform.position;

        if (gameTitle != null)
        {
            StartCoroutine(FlickerTitle());
        }
        
        if (menuCanvasGroup != null)
        {
            menuCanvasGroup.alpha = 0;
            StartCoroutine(FadeInMenu());
        }

        if (moodLight != null)
        {
            StartCoroutine(FlickerLight());
        }
    }

    private void Update()
    {
        // Force lock position every frame to prevent movement
        if (rig != null)
        {
            rig.transform.position = initialRigPos;
        }
    }

    private IEnumerator FadeInMenu()
    {
        float duration = 2.0f;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            menuCanvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
    }

    private IEnumerator FlickerTitle()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 7f));
            int flickers = Random.Range(3, 6);
            for (int i = 0; i < flickers; i++)
            {
                gameTitle.alpha = 0.1f;
                yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
                gameTitle.alpha = 1f;
                yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
            }
        }
    }

    private IEnumerator FlickerLight()
    {
        float baseIntensity = moodLight.intensity;
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            moodLight.intensity = baseIntensity * Random.Range(0.8f, 1.2f);
            if (Random.value > 0.95f)
            {
                moodLight.intensity = 0;
                yield return new WaitForSeconds(0.1f);
                moodLight.intensity = baseIntensity;
            }
        }
    }

    public void StartGame()
    {
        PlayClickSound();
        SceneManager.LoadScene("Game_Act1_TESTVR");
    }

    public void OpenSettings()
    {
        PlayClickSound();
        if (mainButtonsRoot != null) mainButtonsRoot.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayClickSound();
        if (mainButtonsRoot != null) mainButtonsRoot.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void PlayHoverSound()
    {
        if (audioSource != null && hoverSound != null)
            audioSource.PlayOneShot(hoverSound);
    }

    public void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    public void ExitGame()
{
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
