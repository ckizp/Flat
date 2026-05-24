using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    [SerializeField] private float transitionSpeed = 10f;
    
    private Vector3 initialScale;
    private Vector3 targetScale;
    private MainMenuUIManager uiManager;

    private void Start()
    {
        initialScale = transform.localScale;
        targetScale = initialScale;
        uiManager = Object.FindFirstObjectByType<MainMenuUIManager>();
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * transitionSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"[UIButtonHover] Hover Enter: {gameObject.name}");
        targetScale = Vector3.Scale(initialScale, hoverScale);
        if (uiManager != null) uiManager.PlayHoverSound();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"[UIButtonHover] Hover Exit: {gameObject.name}");
        targetScale = initialScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[UIButtonHover] Clicked: {gameObject.name}");
        targetScale = initialScale;
    }
}
