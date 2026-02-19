using UnityEngine;

public class DeploySmoke : MonoBehaviour
{
    [Header("Smoke Settings")]
    [SerializeField] private ParticleSystem smokePrefab;
    [SerializeField] private float smokeDuration = 1.5f;

    private ParticleSystem currentSmoke;

    public void SpawnSmoke()
    {
        if (smokePrefab == null) return;

        // Instantiate the smoke particle system at this object's position
        currentSmoke = Instantiate(smokePrefab, transform.position, Quaternion.identity);
        currentSmoke.transform.parent = transform; // optional: attach to object so it moves with it

        currentSmoke.Play();

        // Stop and destroy after smokeDuration
        Destroy(currentSmoke.gameObject, smokeDuration);
    }
}
