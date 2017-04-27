using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private bool _activated;

	// Use this for initialization
	void Start ()
	{
	    _activated = false;
	    GetComponent<Animator>().enabled = false;
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

            GetComponent<Animator>().enabled = true;
            foreach (var particle in GetComponentsInChildren<ParticleSystem>())
            {
                particle.Play();
            }
            
            GetComponent<AudioSource>().Play();

            var gm = GameManager.Get();
            if (!gm) return;
            gm.SaveToMemory();
            gm.SaveToFiles();
            gm.UseCheckPoint = true;
        }
    }
}
