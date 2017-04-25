﻿using System.Collections;
using System.Collections.Generic;
using Prime31.TransitionKit;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGoal : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var gameManager = GameManager.Get();
            if (gameManager)
            {
                gameManager.BeatLevel(SceneManager.GetActiveScene().name);
                gameManager.SaveToMemory();
                gameManager.SaveToFiles();
                gameManager.UseCheckPoint = false;
            }

            var fishEye = new FishEyeTransition()
            {
                nextScene = "Hub",
                duration = 2.0f,
                size = 0.2f,
                zoom = 100.0f,
                colorSeparation = 0.1f
            };
            TransitionKit.instance.transitionWithDelegate(fishEye);
        }
    }
}
