using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// Forces a world-space UI (e.g. the mission HUD) to always render on top of
/// scene geometry, so it never gets hidden behind walls. Sets the UI/TMP
/// materials' depth test to "Always" and pushes them to the overlay queue.
/// </summary>
public class VRHudOnTop : MonoBehaviour
{
    private const int Always = (int)CompareFunction.Always;

    private void Start()
    {
        // TextMeshPro labels (mission title / step text).
        foreach (var tmp in GetComponentsInChildren<TMP_Text>(true))
        {
            if (tmp.fontMaterial != null)
            {
                // TMP UGUI shaders honour unity_GUIZTestMode like the default UI shader.
                tmp.fontMaterial.SetInt("unity_GUIZTestMode", Always);
                tmp.fontMaterial.SetInt("_ZTestMode", Always); // fallback for variants that expose it
                tmp.fontMaterial.renderQueue = 4000;
            }
        }

        // Plain UI graphics (panel backgrounds, sliders, images).
        foreach (var graphic in GetComponentsInChildren<MaskableGraphic>(true))
        {
            if (graphic is TMP_Text) continue;

            var mat = new Material(graphic.materialForRendering);
            mat.SetInt("unity_GUIZTestMode", Always);
            mat.renderQueue = 4000;
            graphic.material = mat;
        }
    }
}
