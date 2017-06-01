using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// this is the start menu for the game
/// it requires the game manager (to load a saved game, or start a new game)
/// </summary>
public class StartMenu : MonoBehaviour
{
    public GameObject FirstSelectedObject;
    private GameManager _gm;

    private void Awake()
    {
        _gm = GameManager.Instance;
        //RequestFocus();
    }

    public void RequestFocus()
    {
        EventSystem.current.SetSelectedGameObject(FirstSelectedObject);
    }


    public void LoadGame()
    {
        _gm.LoadGame();
    }


    public void NewGame()
    {
        _gm.IsCountingTime = true;
        _gm.GameTime = 0f;
        _gm.NewGame();
    }


    public void ExitGame()
    {
        Application.Quit();
    }


    private void Hide()
    {
        _gm.GetComponent<MenuHandler>().HideMenus();
    }
}
