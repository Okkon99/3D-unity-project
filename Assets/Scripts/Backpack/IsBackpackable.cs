using UnityEngine;

public class IsBackpackable : MonoBehaviour
{
    public Rigidbody rb;
    private Collider[] colliders;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public virtual bool CanEnterBackpack()
    {
        return true;
    }

    public virtual void OnEnterBackpack(Transform anchor)
    {
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        transform.SetParent(anchor);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public virtual void OnExitBackpack(Vector3 force)
    {
        transform.SetParent(null);

        if (rb != null)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            rb.isKinematic = false;
            rb.AddForce(force, ForceMode.VelocityChange);
        }

        foreach (var col in colliders)
        {
            col.enabled = true;
        }
    }
}
