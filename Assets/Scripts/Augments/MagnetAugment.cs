using System.Collections.Generic;
using UnityEngine;

public class MagnetAugment : MonoBehaviour
{
    [Header("Magnet Settings")]
    [SerializeField] private float magnetRadius = 5f;
    [SerializeField] private float pullStrength = 10f;

    [Header("References")]
    [SerializeField] private Transform magnetCenter;

    private bool magnetActive = false;
    private List<Rigidbody> objectsBeingPulled = new List<Rigidbody>();

    private void Update()
    {
        if (InputManager.instance.Input.Gameplay.Augment.triggered)
        {
            magnetActive = !magnetActive;
        }
    }

    private void FixedUpdate()
    {
        if (magnetActive)
        {
            PullObjects();
        }
    }

    private void PullObjects()
    {
        Collider[] colliders = Physics.OverlapSphere(magnetCenter.position, magnetRadius);

        objectsBeingPulled.Clear();

        foreach (var col in colliders)
        {
            if (col.TryGetComponent<Rigidbody>(out var rb))
            {
                if (rb.isKinematic || rb.gameObject == this.gameObject)
                    continue;

                objectsBeingPulled.Add(rb);

                Vector3 direction = (magnetCenter.position - rb.position).normalized;
                rb.AddForce(direction * pullStrength * Time.deltaTime, ForceMode.VelocityChange);
            }
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
