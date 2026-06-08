using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BowAudio : MonoBehaviour
{
    [SerializeField] private AudioClip shootSFX;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);
    [SerializeField] private float fadeInDuration = 0.02f;
    [SerializeField] private float fadeOutDuration = 0.05f;
    [SerializeField] private int voiceCount = 4;

    private AudioSource[] voices;
    private int _nextVoice;

    void Awake()
    {
        voiceCount = Mathf.Max(1, voiceCount);
        voices = new AudioSource[voiceCount];

        voices[0] = GetComponent<AudioSource>();
        for (int i = 1; i < voiceCount; i++)
            voices[i] = gameObject.AddComponent<AudioSource>();

        foreach (AudioSource voice in voices)
        {
            voice.playOnAwake = false;
            voice.loop = false;
        }
    }

    public void PlayShootSFX()
    {
        AudioSource voice = NextFreeVoice();
        SfxPlayer.Play(this, voice, shootSFX, volume, pitchRange, fadeInDuration, fadeOutDuration);
    }

    private AudioSource NextFreeVoice()
    {
        for (int i = 0; i < voices.Length; i++)
        {
            AudioSource voice = voices[(_nextVoice + i) % voices.Length];
            if (!voice.isPlaying)
            {
                _nextVoice = (System.Array.IndexOf(voices, voice) + 1) % voices.Length;
                return voice;
            }
        }

        AudioSource fallback = voices[_nextVoice];
        _nextVoice = (_nextVoice + 1) % voices.Length;
        return fallback;
    }
}
