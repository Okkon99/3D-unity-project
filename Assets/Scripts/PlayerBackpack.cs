using Unity.VisualScripting;
using UnityEngine;

public class PlayerBackpack : MonoBehaviour
{
    [SerializeField] private Transform backpackAnchor;

    private GameObject currentItem;

    public bool IsOccupied => currentItem != null;

    public GameObject CurrentItem => currentItem;

    public bool TryInsert(GameObject item)
    {
        if (IsOccupied) return false;

        currentItem = item;

        Vector3 startPos = item.transform.position;


        item.transform.SetParent(backpackAnchor);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        return true;
    }

    public GameObject Deploy(Vector3 force)
    {
        if (!IsOccupied) return null;

        GameObject item = currentItem;
        currentItem = null;

        item.transform.SetParent(null);

        if (item.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.AddForce(force, ForceMode.VelocityChange);
        }

        return item;
    }
}
