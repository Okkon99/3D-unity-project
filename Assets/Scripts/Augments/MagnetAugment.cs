using UnityEngine;

public class MagnetAugment : AugmentBase
{
    [Header("Magnet Settings")]
    [SerializeField] private float magnetRadius = 5f;
    [SerializeField] private float pullStrength = 10f;

    [Header("References")]
    [SerializeField] private Transform magnetCenter;



    private void FixedUpdate()
    {
        if (!isActive || owner == null)
            return;
        
        PullObjects();
    }

    private void PullObjects()
    {
        Collider[] colliders = Physics.OverlapSphere(magnetCenter.position, magnetRadius);

        foreach (var col in colliders)
        {
            if (!col.TryGetComponent<Rigidbody>(out var rb))
                continue;
            
            if (rb.isKinematic)
                continue;

            if (rb.GetComponent<PlayerMovement>())
                continue;

            Vector3 direction = (magnetCenter.position - rb.position).normalized;
            rb.AddForce(direction * pullStrength, ForceMode.Acceleration);

        }
    }

    private void OnDrawGizmosSelected()
    {
        if (magnetCenter != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(magnetCenter.position, magnetRadius);
        }
    }
}
