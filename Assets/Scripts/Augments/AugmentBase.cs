using UnityEngine;

public abstract class AugmentBase : MonoBehaviour
{

    public bool isActive;
    protected PlayerMovement owner;
    protected Vector3 startPosition;
    protected Quaternion startRotation;
    private BackpackPreviewRenderer previewRenderer;

    protected Rigidbody rb;
    Color32 amber;
    Color32 amberGlow;
    
    Color32 green;
    Color32 greenGlow;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        previewRenderer = FindFirstObjectByType<BackpackPreviewRenderer>();
        amber = new Color32(255, 191, 0, 255);
        amberGlow = new Color32(255, 133, 0, 255);
        green = new Color32(0, 255, 0, 255);
        greenGlow = new Color32(0, 210, 0, 255);
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

        transform.position = new Vector3(startPosition.x, startPosition.y - 1f, startPosition.z);
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

        BackpackPreviewRenderer pr = previewRenderer;
        pr.augmentSlotCurrentStatusText.color = isActive ? green : amber;
        pr.augmentSlotCurrentStatusText.fontMaterial.SetColor("_GlowColor", isActive ? greenGlow : amberGlow);
    }

    public virtual void Toggle()
    {
        BackpackPreviewRenderer pr = previewRenderer;
        isActive = !isActive;
        pr.augmentSlotCurrentStatusText.text = isActive ? "(Active)" : "(Inactive)";
        pr.augmentSlotCurrentStatusText.color = isActive ? green : amber;
        pr.augmentSlotCurrentStatusText.fontMaterial.SetColor("_GlowColor", isActive ? greenGlow : amberGlow);
    }
}
