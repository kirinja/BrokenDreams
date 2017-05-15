using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MusicTrigger : MonoBehaviour
{
    public AudioClip MusicClip;
    public float TransitionTime = 1f;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
            GameManager.Instance.PlayMusic(MusicClip, TransitionTime);
    }
}
