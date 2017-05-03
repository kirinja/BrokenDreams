using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip MusicClip;

	// Use this for initialization
	void Start ()
    {
		GameManager.Instance.PlayMusic(MusicClip);
	}
}
