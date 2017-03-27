using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : MonoBehaviour
{
    public bool Inverted;
    public string Scene;

	// Use this for initialization
	void Start () {
        if (!GameObject.FindObjectOfType<GameManager>().LevelAvailable(Scene) && !Inverted || GameObject.FindObjectOfType<GameManager>().LevelAvailable(Scene) && Inverted)
        {
            enabled = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
