using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGoal : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            gameManager.BeatLevel(SceneManager.GetActiveScene().name);
            gameManager.SaveToMemory();
            gameManager.SaveToFiles();
            SceneManager.LoadScene("Hub");
        }
    }
}
