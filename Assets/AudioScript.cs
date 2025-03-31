using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AudioScript : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private float volume = 1f;
    [SerializeField] private bool playOnAwake = true;

    private ParticleSystem particleSystem;
    private AudioSource audioSource;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();

        // Set up audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = spawnSound;
        audioSource.volume = volume;

        if (playOnAwake && spawnSound != null)
        {
            PlaySpawnSound();
        }
    }

    void OnEnable()
    {
        // Subscribe to the particle system's play event
        if (spawnSound != null)
        {
            particleSystem.Play();
            PlaySpawnSound();
        }
    }

    public void PlaySpawnSound()
    {
        if (spawnSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }
}