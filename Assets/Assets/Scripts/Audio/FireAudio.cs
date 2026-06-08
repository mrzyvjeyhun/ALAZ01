using UnityEngine;

public class FireAudio : MonoBehaviour
{
    [SerializeField] private AudioClip fireMusic;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (fireMusic != null)
        {
            audioSource.clip = fireMusic;
        }

        audioSource.loop = true;

        if (audioSource.clip != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void OnEnable()
    {
        if (audioSource != null && audioSource.clip != null && !audioSource.isPlaying)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
