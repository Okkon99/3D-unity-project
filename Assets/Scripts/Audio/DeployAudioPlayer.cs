using System.Collections;
using UnityEngine;

public class DeployAudioPlayer : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource source;

    [Header("Clips")]
    [SerializeField] private AudioClip[] metalImpact;
    [SerializeField] private AudioClip steamHiss;


    [Header("Timing")]
    [SerializeField] private float hissDelay = 0.25f;

    Coroutine currentRoutine;

    public void PlayDeploySequence()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(DeployRoutine());
    }

    private IEnumerator DeployRoutine()
    {
        if (metalImpact.Length > 0)
        {
            var clip = metalImpact[Random.Range(0, metalImpact.Length)];
            source.PlayOneShot(clip);
        }

        yield return new WaitForSeconds(hissDelay);

        if (steamHiss != null)
            source.PlayOneShot(steamHiss);
    }
}
