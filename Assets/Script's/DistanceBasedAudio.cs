using UnityEngine;

public class DistanceBasedAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public Transform player;
    public float maxDistance = 5f;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (audioSource != null)
        {

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    void Update()
    {
        if (player == null || audioSource == null) return;

        float distance = Vector2.Distance(player.position, transform.position);
        float volume = Mathf.Clamp01(1f - (distance / maxDistance));

        audioSource.volume = volume;
    }
}