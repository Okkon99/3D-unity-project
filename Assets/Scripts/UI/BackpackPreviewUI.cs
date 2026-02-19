using UnityEngine;

public class BackpackPreviewUI : MonoBehaviour
{
    [SerializeField] private Transform previewStage;
    [SerializeField] private Transform previewPivot;
    [SerializeField] private Camera previewCamera;

    private GameObject currentPreview;

    public void ShowItem(GameObject itemPrefab)
    {
        ClearPreview();

        currentPreview = Instantiate(itemPrefab, previewPivot);

        SetLayerRecursive(currentPreview, LayerMask.NameToLayer("PreviewItem"));

        // disable gameplay components
        foreach (var rb in currentPreview.GetComponentsInChildren<Rigidbody>())
            Destroy(rb);

        foreach (var col in currentPreview.GetComponentsInChildren<Collider>())
            Destroy(col);

        // 45° presentation angle
        previewPivot.localRotation = Quaternion.Euler(25f, 45f, 0f);
    }

    public void ClearPreview()
    {
        if (currentPreview != null)
            Destroy(currentPreview);
    }

    void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
            SetLayerRecursive(child.gameObject, layer);
    }
}
