using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform grabManager;
    [SerializeField] private Transform cameraAnchor;

    [Header("Grab Settings")]
    [SerializeField] private float maxReach = 4f;
    [SerializeField] private float holdDistance = 2f;

    private Rigidbody heldBody;
    private Transform heldTransform;
    bool isHolding;


    void Update()
    {
        if (!GetComponent<PlayerMovement>().isActivePlayer)
        {
            return;
        }

        var input = InputManager.instance.Input.Gameplay;


        UpdateGrabManagerTransform();

        if (input.Grab.triggered)
        {
            if (heldBody == null)
                TryGrab();
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

    private void TryGrab()
    {
        Ray ray = new Ray(cameraAnchor.position, cameraAnchor.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, maxReach))
            return;

        if (!hit.collider.CompareTag("Grabbable"))
        {
            return;
        }

        if (hit.collider.TryGetComponent<ButtonScript>(out ButtonScript button))
        {
            button.OnButtonPressed();
        }

        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null)
            return;

        heldBody = rb;
        heldTransform = rb.transform;
        isHolding = true;

        rb.isKinematic = true;

        heldTransform.SetParent(grabManager);
        heldTransform.localPosition = Vector3.zero;
        heldTransform.localRotation = Quaternion.identity;
    }

    private void Release()
    {
        heldTransform.SetParent(null);
        heldBody.isKinematic = false;

        heldBody = null;
        heldTransform = null;
        isHolding = false;
    }
}
