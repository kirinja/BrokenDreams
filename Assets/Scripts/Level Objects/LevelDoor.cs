using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : MonoBehaviour
{
    public bool Inverted;
    public string Scene;

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(Inverted ? 
                !GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>().LevelAvailable(Scene):
                GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GameManager>().LevelAvailable(Scene));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
