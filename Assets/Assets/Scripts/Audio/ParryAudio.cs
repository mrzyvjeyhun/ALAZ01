using UnityEngine;

public class ParryAudio : MonoBehaviour
{
    [SerializeField] private AudioClip perfectParrySFX;
    [SerializeField] private AudioClip yellowParrySFX;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);
    [SerializeField] private float fadeInDuration = 0.02f;
    [SerializeField] private float fadeOutDuration = 0.05f;

    private AudioSource audioSource;
    private Coroutine _sfxRoutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    public void PlayPerfectSFX()
    {
        if (_sfxRoutine != null) StopCoroutine(_sfxRoutine);
        _sfxRoutine = SfxPlayer.Play(this, audioSource, perfectParrySFX, volume, pitchRange, fadeInDuration, fadeOutDuration);
    }

    public void PlayYellowSFX()
    {
        if (_sfxRoutine != null) StopCoroutine(_sfxRoutine);
        _sfxRoutine = SfxPlayer.Play(this, audioSource, yellowParrySFX, volume, pitchRange, fadeInDuration, fadeOutDuration);
    }
}
