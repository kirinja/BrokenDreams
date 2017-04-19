using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject FirstSelectedObject;

	// Use this for initialization
	void Start ()
    {
	}

    // Update is called once per frame
    void Update()
    {
    }

    public void RequestFocus()
    {
        EventSystem.current.SetSelectedGameObject(FirstSelectedObject);
    }

    public void ResumeGame()
    {
        var gameManager = GameManager.Get();
        if (gameManager)
        {
            gameManager.Pause();
        }
    }

    public void GoToHub()
    {
        SceneManager.LoadScene("Hub");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
