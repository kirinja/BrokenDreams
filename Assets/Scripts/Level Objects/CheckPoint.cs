using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private bool _activated;

	// Use this for initialization
	void Start ()
	{
	    _activated = false;
        transform.Find("Checkpoint_circle").GetComponent<ParticleSystem>().Stop();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_activated)
        {
            _activated = true;
            other.GetComponent<Controller3D>().SetSpawn(transform.position);
            transform.Find("Checkpoint_circle").GetComponent<ParticleSystem>().Play();
            GetComponent<AudioSource>().Play();

            var gm = GameManager.Get();
            if (!gm) return;
            gm.SaveToMemory();
            gm.SaveToFiles();
            gm.UseCheckPoint = true;
        }
    }
}
