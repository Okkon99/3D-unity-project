using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerGrab : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform grabManager;
    [SerializeField] private Transform cameraAnchor;
    [SerializeField] private Rigidbody playerRB;
    [SerializeField] private TextMeshProUGUI crosshairEdges;
    [SerializeField] private TextMeshProUGUI crosshairText;

    [Header("Grab Settings")]
    [SerializeField] private float maxReach = 4f;
    [SerializeField] private float holdDistance = 2f;

    public Rigidbody heldBody;
    private Transform heldTransform;
    bool isHolding;

    Color edgeColor;
    Color textColor;
    
    private void Awake()
    {
        edgeColor = crosshairEdges.color;
        textColor = crosshairText.color;
    }


    void Update()
    {
        if (!GetComponent<PlayerMovement>().isActivePlayer)
            return;

        var input = InputManager.instance.Input.Gameplay;

        UpdateGrabManagerTransform();

        bool hasTarget = CheckForGrabbable(out RaycastHit hit);
        UpdateUI(hasTarget);

        if (input.Grab.triggered)
        {
            if (heldBody == null)
                TryGrab(hit);
            else
                Release();
        }
    }


    private void UpdateGrabManagerTransform()
    {
        grabManager.position = cameraAnchor.position +
                               cameraAnchor.forward * holdDistance;

        grabManager.rotation = cameraAnchor.rotation;
    }

    private void TryGrab(RaycastHit hit)
    {
        if (!hit.collider) return;

        if (hit.collider.TryGetComponent<ButtonScript>(out ButtonScript button))
            button.OnButtonPressed();

        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null)
            return;

        heldBody = rb;
        heldTransform = rb.transform;

        rb.isKinematic = true;

        heldTransform.SetParent(grabManager);
        heldTransform.localPosition = Vector3.zero;
        heldTransform.localRotation = Quaternion.identity;
    }

    public void Release()
    {
        heldTransform.SetParent(null);
        heldBody.isKinematic = false;

        heldBody = null;
        heldTransform = null;
        isHolding = false;
    }

    // UI
    void UpdateUI(bool hasTarget)
    {
        if (heldBody != null)
        {
            SetAlpha(1f);
            SetText("release");
            crosshairEdges.text = "[             ]";
        }
        else if (hasTarget)
        {
            SetAlpha(1f);
            SetText("grab");
            crosshairEdges.text = "[        ]";
        }
        else
        {
            SetAlpha(edgeColor.a);
            SetText("");
            crosshairEdges.text = "[        ]";
        }
    }

    bool CheckForGrabbable(out RaycastHit hit)
    {
        Ray ray = new Ray(cameraAnchor.position, cameraAnchor.forward);

        if (!Physics.Raycast(ray, out hit, maxReach))
            return false;

        return hit.collider.CompareTag("Grabbable");
    }

    public void SetAlpha(float alpha) // 0–1
    {
        Color c = crosshairEdges.color;
        c.a = alpha;
        crosshairEdges.color = c;
    }

    public void SetText(string text)
    {
        crosshairText.text = text;
    }
}
