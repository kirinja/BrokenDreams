using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossDoor : MonoBehaviour
{
    public string Scene;
    public bool ShouldInitialize = true;

	// Use this for initialization
	void Start () {
        if (ShouldInitialize)
        {
            GetComponent<Animator>().enabled = false;

	        var open = GameManager.Instance.LevelAvailable(Scene);
            if (open)
            {
                var parent1 = transform.Find("Trapdoor parent 1");
                parent1.localEulerAngles = new Vector3(parent1.localRotation.x, parent1.localRotation.y, -90);

                var parent2 = transform.Find("Trapdoor parent 2");
                parent2.localEulerAngles = new Vector3(parent2.localRotation.x, parent2.localRotation.y, -90);
            }
        }
    }


    public void Play()
    {
        GetComponent<Animator>().enabled = true;
    }
}
