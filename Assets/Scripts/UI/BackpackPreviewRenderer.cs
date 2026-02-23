using TMPro;
using UnityEngine;

public class BackpackPreviewRenderer : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform previewAnchor;
    [SerializeField] private Vector3 displayAngle;
    [SerializeField] private TextMeshProUGUI augmentSlotText;
    [SerializeField] public TextMeshProUGUI augmentSlotCurrentStatusText;

    private GameObject currentPreviewInstance;
    private AugmentBase currentItemAugmentBase;

    public void ShowItem(IsBackpackable item)
    {
        Clear();

        if (item == null) return;

        currentPreviewInstance = Instantiate(item.gameObject, previewAnchor);

        foreach (var smoke in currentPreviewInstance.GetComponentsInChildren<ParticleSystem>())
            Destroy(smoke.gameObject);//prevents smoke from showing up in the UI view if picked up early enough

        currentPreviewInstance.transform.localPosition = Vector3.zero;
        currentPreviewInstance.transform.localRotation =
            Quaternion.Euler(displayAngle); 
        currentPreviewInstance.transform.localScale = Vector3.one;

        DisablePhysics(currentPreviewInstance);

        SetLayerRecursively(currentPreviewInstance, LayerMask.NameToLayer("PreviewLayer"));

        if (!item.GetComponent<PlayerMovement>())
        {
            augmentSlotText.text = item.gameObject.name.ToString();
            if (item.GetComponent<AugmentBase>())
            {
                if (item.GetComponent<AugmentBase>())
                {
                    currentItemAugmentBase = item.GetComponent<AugmentBase>();
                    augmentSlotCurrentStatusText.text = currentItemAugmentBase.isActive ? "(Active)" : "(Inactive)";
                }
            }
        }
        else
            augmentSlotText.text = "AUGMENT UNKNOWN!";
    }

    public void Clear()
    {
        if (currentPreviewInstance != null)
        {
            Destroy(currentPreviewInstance);
            augmentSlotText.text = "NO AUGMENT.";
            augmentSlotCurrentStatusText.text = "";
        }
    }

    void DisablePhysics(GameObject obj)
    {
        foreach (var rb in obj.GetComponentsInChildren<Rigidbody>())
            Destroy(rb);

        foreach (var col in obj.GetComponentsInChildren<Collider>())
            Destroy(col);

        foreach (var script in obj.GetComponentsInChildren<MonoBehaviour>())
        {
            if (script is IsBackpackable) continue;
            Destroy(script);
        }
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
