using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDoor : MonoBehaviour
{
    public string Scene;
    public bool RequiresInput = true;

    private void Start()
    {
        if (!GameObject.FindObjectOfType<GameManager>().LevelAvailable(Scene))
        {
            enabled = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && (!RequiresInput || Input.GetAxisRaw("Vertical") >= 0.5f))
        {
            SceneManager.LoadScene(Scene);
        }
    }
}
