using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    public GameObject FirstSelectedObject;


    public void RequestFocus()
    {
        EventSystem.current.SetSelectedGameObject(FirstSelectedObject);
    }


    public void ResumeGame()
    {
        Hide();
    }


    public void GoToHub()
    {
        Hide();
        SceneManager.LoadScene("Hub");
    }


    public void ExitGame()
    {
        Application.Quit();
    }


    private void Hide()
    {
        var gameManager = GameManager.Get();
        if (gameManager)
            gameManager.GetComponent<MenuHandler>().HideMenus();
    }
}