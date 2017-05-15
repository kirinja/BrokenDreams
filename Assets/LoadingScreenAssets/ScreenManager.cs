// ************************************************************************ 
// File Name:   ScreenManager.cs 
// Purpose:    	Transfers between scenes
// Project:		Framework
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 

using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


// ************************************************************************ 
// Class: ScreenManager
// ************************************************************************
public class ScreenManager : MonoBehaviour {
	
	
	// ********************************************************************
	// Exposed Data Members 
	// ********************************************************************
	[SerializeField]
	private FadeSprite m_blackScreenCover;
	
	
	// ********************************************************************
	// Function:	Update()
	// Purpose:		Called once per frame.
	// ********************************************************************
	void Update()
	{
		//if (Input.GetMouseButtonDown(0))
		//{
		//	StartCoroutine(LoadSceneAsync("GameScreen"));
		//}
	}
	
	
	// ********************************************************************
	// Function:	LoadScene()
	// Purpose:		Loads the supplied scene
	// ********************************************************************
	public IEnumerator LoadSceneAsync(string sceneName, float minDuration)
	{
		//// Fade to black
		//yield return StartCoroutine(m_blackScreenCover.FadeIn());
		
		//// Load loading screen
		//yield return SceneManager.LoadSceneAsync("LoadingScreen");
		
		//// !!! unload old screen (automatic)
		
        
		//// Fade to loading screen
		//yield return StartCoroutine(m_blackScreenCover.FadeOut());
		
		float endTime = Time.time + minDuration;

	    yield return new WaitForSeconds(minDuration);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        async.allowSceneActivation = false;
        
        while (!async.isDone)
        {
            // [0, 0.9] > [0, 1]
            float progress = Mathf.Clamp01(async.progress / 0.9f);
            //Debug.Log("Loading progress: " + (progress * 100) + "%");
            //LoadingText.color = new Color(LoadingText.color.r, LoadingText.color.g, LoadingText.color.b, Mathf.PingPong(Time.time, 1));

            // Loading completed
            if (Math.Abs(async.progress - 0.9f) < 0.01f)
            {
                //Debug.Log("Press a key to start");
                //if (Input.anyKey)

                //GameManager.Instance.FadeIn();
                async.allowSceneActivation = true;

                yield return StartCoroutine(m_blackScreenCover.FadeIn());
            }

            //yield return null;
        }

        // Load level async
        //yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        
	    while (Time.time < endTime)
	    {
	        yield return null;
	    }

	    // Play music or perform other misc tasks
		
		// Fade to black
		//yield return StartCoroutine(m_blackScreenCover.FadeIn());
		
		// !!! unload loading screen
		LoadingSceneManager.UnloadLoadingScene();
		
		// Fade to new screen
		yield return StartCoroutine(m_blackScreenCover.FadeOut());
	}
    
    public void LoadLevel(string sceneName, float minTime)
    {
        StartCoroutine(LoadSceneAsync(sceneName, minTime));
    }

}
