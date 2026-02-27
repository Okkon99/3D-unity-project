using System.Collections;
using UnityEngine;

public class MagnetHum : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource source;

    [Header("Clip")]
    [SerializeField] private AudioClip magnetHum;

    [Header("Settings")]
    [SerializeField] private float fadeDuration;
    [SerializeField] private float randomStartWindow;

    private Coroutine fadeRoutine;


    private void Awake()
    {
        source.clip = magnetHum;
        source.loop = true;
        source.playOnAwake = false;
    }

    public void PlayHumming()
    {
        if (source.isPlaying) return;

        float maxStart = Mathf.Min(randomStartWindow, magnetHum.length - 0.1f);
        source.time = Random.Range (0f, maxStart);

        source.volume = 0f;
        source.Play();

        StartFade(1f);
    }

    public void StopHumming()
    {
        if (!source.isPlaying) return;

        StartFade(0f);
    }

    private void StartFade(float targetVolume)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(targetVolume));
    }

    private IEnumerator FadeRoutine(float targetVolume)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / fadeDuration);
            yield return null;
        }

        source.volume = targetVolume;

        if (Mathf.Approximately(targetVolume, 0f))
            source.Stop();
    }
}
