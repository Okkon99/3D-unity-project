using UnityEngine;

public abstract class AugmentBase : MonoBehaviour
{
    protected bool isActive;
    protected PlayerMovement owner;
    protected Vector3 startPosition;
    protected Quaternion startRotation;

    protected Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Deathbox"))
        {
            ResetToSpawn();
        }
    }

    public virtual void ResetToSpawn()
    {
        transform.SetParent(null);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = new Vector3(startPosition.x, startPosition.y - 2f, startPosition.z);
        transform.rotation = startRotation;

        gameObject.SetActive(true);
    }

    public virtual void OnEquipped(PlayerMovement newOwner)
    {
        owner = newOwner;
        isActive = false;
        enabled = true;
    }

    public virtual void OnUnequipped()
    {
        isActive = false;
        owner = null;
        enabled = false;
    }

    public virtual void Toggle()
    {
        isActive = !isActive;
    }
}
