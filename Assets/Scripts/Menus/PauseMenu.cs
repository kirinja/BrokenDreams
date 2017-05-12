using Prime31.TransitionKit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    public GameObject FirstSelectedObject;


    public void RequestFocus()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(FirstSelectedObject);
    }


    public void ResumeGame()
    {
        Hide();
    }


    public void GoToHub()
    {
        Hide();
        if (SceneManager.GetActiveScene().name.Equals("Hub")) return;

        var maskTex = GameManager.Instance.MaskTexture;
        GameManager.Instance.NextLevelToLoad = "Hub";
        var mask = new ImageMaskTransition()
        {
            maskTexture = maskTex,
            backgroundColor = Color.black,
            duration = 1.0f,
            nextScene = "LoadingScreen"
        };
        TransitionKit.instance.transitionWithDelegate(mask);
        //SceneManager.LoadScene("Hub");
    }


    public void ExitGame()
    {
        Application.Quit();
    }


    private void Hide()
    {
        var gameManager = GameManager.Instance;
        gameManager.GetComponent<MenuHandler>().HideMenus();
    }
}