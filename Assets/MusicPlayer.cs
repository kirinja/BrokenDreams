using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip MusicClip;
    public float TransitionTime = 2f;

	// Use this for initialization
	void Start ()
    {
		GameManager.Instance.PlayMusic(MusicClip, TransitionTime);
	}
}
