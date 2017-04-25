using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuHandler : MonoBehaviour
{
    public GameObject PauseMenu;

    private GameObject _pauseMenu;


    private void Awake()
    {
        _pauseMenu = Instantiate(PauseMenu);
        DontDestroyOnLoad(_pauseMenu);
    }


    // Use this for initialization
    private void Start()
    {
        _pauseMenu.SetActive(false);
    }


    public void ShowPauseMenu()
    {
        GetComponent<GameManager>().Paused = true;
        _pauseMenu.SetActive(true);
        _pauseMenu.GetComponent<PauseMenu>().RequestFocus();
    }


    public void HideMenus()
    {
        GetComponent<GameManager>().Paused = false;
        _pauseMenu.SetActive(false);
    }
}