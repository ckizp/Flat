using Flat.Gameplay.Inventory;
using UnityEngine;

namespace Flat.Gameplay.Inventory.Implementations
{
    [CreateAssetMenu(fileName = "New Flashlight", menuName = "Flat/Inventory/Flashlight")]
    public class FlashlightItem : Item
    {
        private bool isActive = false;

        public override void Use()
        {
            isActive = !isActive;
        }
        
        public override void Use(GameObject heldItemInstance)
        {
            isActive = !isActive;
            
            if (heldItemInstance != null)
            {
                Light[] allLights = heldItemInstance.GetComponentsInChildren<Light>(true);

                if (allLights.Length > 0)
                {
                    foreach (Light light in allLights)
                    {
                        light.enabled = isActive;
                    }
                }
                else
                {
                    Transform spotLightTransform = heldItemInstance.transform.Find("SpotLight");
                    if (spotLightTransform != null)
                    {
                        Light spotLight = spotLightTransform.GetComponent<Light>();
                        if (spotLight != null)
                        {
                            spotLight.enabled = isActive;
                        }
                    }
                }
            }
        }
    }
}
