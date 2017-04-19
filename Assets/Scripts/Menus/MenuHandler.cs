using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    public GameObject PauseMenu;

    private GameObject pauseMenu;

    private void Awake()
    {
        pauseMenu = Instantiate(PauseMenu);
        DontDestroyOnLoad(pauseMenu);
    }

	// Use this for initialization
	private void Start ()
	{
        pauseMenu.SetActive(false);
    }
    
    public void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
        pauseMenu.GetComponent<PauseMenu>().RequestFocus();
    }

    public void HideMenus()
    {
        pauseMenu.SetActive(false);
    }
}
