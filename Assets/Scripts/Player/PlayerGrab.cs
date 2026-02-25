using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerGrab : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraAnchor;
    [SerializeField] private Rigidbody playerRB;
    [SerializeField] private TextMeshProUGUI crosshairEdges;
    [SerializeField] private TextMeshProUGUI crosshairText;
    [SerializeField] private Collider otherRobotCol;
    [SerializeField] private PlayerGrab otherRobotGrab;

    [Header("Grab Settings")]
    [SerializeField] private float maxReach = 4f;
    [SerializeField] private float holdDistance = 2f;
    [SerializeField] float followStrength = 800f;
    [SerializeField] float damping = 50f;
    [SerializeField] float rotationStrength = 600f;
    [SerializeField] float rotationDamping = 50f;


    public Rigidbody heldBody;
    private Transform heldTransform;
    public bool isHolding;

    Color edgeColor;
    Color textColor;

    private enum TargetType
    {
        None,
        Grabbable,
        Pressable
    }

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


        TargetType targetType = CheckForTarget(out RaycastHit hit);
        UpdateUI(targetType);

        if (input.Grab.triggered)
        {
            if (heldBody == null)
            {
                if (targetType == TargetType.Grabbable)
                    TryGrab(hit);

                else if (targetType == TargetType.Pressable)
                    TryPress(hit);
            }
            else Release();
        }
    }

    private void FixedUpdate()
    {
        if (heldBody == null) return;

        Vector3 targetPos = cameraAnchor.position + cameraAnchor.forward * holdDistance;

        Vector3 toTarget = targetPos - heldBody.position;
        Vector3 force = toTarget * followStrength - heldBody.linearVelocity * damping;
        heldBody.AddForce(force, ForceMode.Acceleration);

        Quaternion targetRot = cameraAnchor.rotation;
        Quaternion delta = targetRot * Quaternion.Inverse(heldBody.rotation);

        delta.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f) angle -= 360f;

        Vector3 torque = axis * angle * Mathf.Deg2Rad * rotationStrength
                         - heldBody.angularVelocity * rotationDamping;

        heldBody.AddTorque(torque, ForceMode.Acceleration);
    }




    private void TryGrab(RaycastHit hit)
    {
        if (otherRobotGrab.heldBody == GetComponent<Rigidbody>())
            return;

        if (!hit.collider) return;

        if (hit.collider.CompareTag("Grabbable"))
        {
            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb == null)
                return;

            if (rb == otherRobotGrab.heldBody)
                otherRobotGrab.Release();

            heldBody = rb;
            Collider heldBodyCol = heldBody.GetComponent<Collider>();
            Physics.IgnoreCollision(heldBodyCol, GetComponent<Collider>(), true);
            Physics.IgnoreCollision(heldBodyCol, otherRobotCol, true);

            isHolding = true;
        }
        else if (hit.collider.CompareTag("Player"))
        {
            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb == null)
                return;

            heldBody = rb;
            Physics.IgnoreCollision(heldBody.GetComponent<Collider>(), GetComponent<Collider>(), true);

            isHolding = true;
        }
    }

    private void TryPress(RaycastHit hit)
    {
        if (hit.collider.TryGetComponent<ButtonScript>(out ButtonScript button))
            button.OnButtonPressed();
    }

    public void Release()
    {
        if (heldBody == null) return;

        Collider heldBodyCol = heldBody.GetComponent<Collider>();
        Physics.IgnoreCollision(heldBodyCol, GetComponent<Collider>(), false);
        Physics.IgnoreCollision(heldBodyCol, otherRobotCol, false);
        heldBody.linearVelocity = Vector3.zero;
        heldBody.angularVelocity = Vector3.zero;
        heldBody = null;
        isHolding = false;
    }

    // UI
    void UpdateUI(TargetType targetType)
    {
        if (heldBody != null)
        {
            SetAlpha(1f);
            SetText("release");
            crosshairEdges.text = "[             ]";
            return;
        }

        switch (targetType)
        {
            case TargetType.Grabbable:
                SetAlpha(1f);
                SetText("grab");
                crosshairEdges.text = "[        ]";
                break;

            case TargetType.Pressable:
                SetAlpha(1f);
                SetText("press");
                crosshairEdges.text = "[          ]";
                break;

            default:
                SetAlpha(edgeColor.a);
                SetText("");
                crosshairEdges.text = "[        ]";
                break;
        }
    }

    TargetType CheckForTarget(out RaycastHit hit)
    {
        Ray ray = new Ray(cameraAnchor.position, cameraAnchor.forward);

        if (!Physics.Raycast(ray, out hit, maxReach))
            return TargetType.None;

        if (hit.collider.CompareTag("Grabbable"))
            return TargetType.Grabbable;

        if (hit.collider.CompareTag("Player"))
            return TargetType.Grabbable;

        if (hit.collider.CompareTag("Pressable"))
            return TargetType.Pressable;

        return TargetType.None;
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
