﻿// ************************************************************************ 
// File Name:   ScreenManager.cs 
// Purpose:    	Transfers between scenes
// Project:		Framework
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


// ************************************************************************ 
// Class: LoadingSceneManager
// ************************************************************************
public class LoadingSceneManager : SingletonTemp<LoadingSceneManager> 
{

	// ********************************************************************
	// Function:	UnloadLoadingScene()
	// Purpose:		Destroys the loading scene
	// ********************************************************************
	public static void UnloadLoadingScene()
	{
		GameObject.Destroy(instance.gameObject);
        //Destroy(instance);
	    SceneManager.UnloadSceneAsync("LoadingScreen");
	}
}
