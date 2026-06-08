using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CompanionAudio : MonoBehaviour
{
    [SerializeField] private AudioClip teleportSFX;
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

    public void PlayTeleportSFX()
    {
        if (_sfxRoutine != null) StopCoroutine(_sfxRoutine);
        _sfxRoutine = SfxPlayer.Play(this, audioSource, teleportSFX, volume, pitchRange, fadeInDuration, fadeOutDuration);
    }
}
