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
        var maskTex = GameManager.Instance.MaskTexture;
        var mask = new ImageMaskTransition()
        {
            maskTexture = maskTex,
            backgroundColor = Color.black,
            duration = 1.5f,
            nextScene = "Hub"
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