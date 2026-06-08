using System.Collections;
using UnityEngine;

public static class SfxPlayer
{
    public static Coroutine Play(MonoBehaviour host, AudioSource source, AudioClip clip,
                                 float volume, Vector2 pitchRange, float fadeIn, float fadeOut)
    {
        if (host == null || source == null || clip == null) return null;
        return host.StartCoroutine(PlayRoutine(source, clip, volume, pitchRange, fadeIn, fadeOut));
    }

    private static IEnumerator PlayRoutine(AudioSource source, AudioClip clip,
                                           float volume, Vector2 pitchRange, float fadeIn, float fadeOut)
    {
        source.clip = clip;
        source.pitch = Random.Range(pitchRange.x, pitchRange.y);
        source.volume = 0f;
        source.Play();

        float duration = clip.length / Mathf.Max(0.01f, source.pitch);
        fadeIn = Mathf.Max(0f, fadeIn);
        fadeOut = Mathf.Max(0f, fadeOut);

        if (fadeIn + fadeOut > duration && fadeIn + fadeOut > 0f)
        {
            float scale = duration / (fadeIn + fadeOut);
            fadeIn *= scale;
            fadeOut *= scale;
        }

        float t = 0f;
        while (t < fadeIn)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, volume, t / fadeIn);
            yield return null;
        }
        source.volume = volume;

        float hold = duration - fadeIn - fadeOut;
        if (hold > 0f) yield return new WaitForSeconds(hold);

        t = 0f;
        while (t < fadeOut)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(volume, 0f, t / fadeOut);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();
    }
}
