using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioClip parrySFX;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);
    [SerializeField] private float fadeInDuration = 0.02f;
    [SerializeField] private float fadeOutDuration = 0.05f;

    [SerializeField] private AudioClip footstepsSFX;
    [Range(0f, 1f)]
    [SerializeField] private float footstepsVolume = 1f;

    private AudioSource audioSource;
    private AudioSource footstepsSource;
    private Coroutine _sfxRoutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        footstepsSource = gameObject.AddComponent<AudioSource>();
        footstepsSource.playOnAwake = false;
        footstepsSource.loop = true;
        footstepsSource.clip = footstepsSFX;
        footstepsSource.volume = footstepsVolume;
    }

    public void PlayParrySFX()
    {
        if (_sfxRoutine != null) StopCoroutine(_sfxRoutine);
        _sfxRoutine = SfxPlayer.Play(this, audioSource, parrySFX, volume, pitchRange, fadeInDuration, fadeOutDuration);
    }

    public void StartFootsteps()
    {
        if (footstepsSFX != null && !footstepsSource.isPlaying)
            footstepsSource.Play();
    }

    public void StopFootsteps()
    {
        if (footstepsSource.isPlaying)
            footstepsSource.Stop();
    }
}
