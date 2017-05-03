using System.Collections;
using System.Collections.Generic;
using Prime31.TransitionKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    public float SplashTimer = 2.0f;
    private float _internalTimer = 0;
    public Image SplashImage;

    private float SplashOutTimer = 1.0f;
    private float _splashOut = 0;
	// Use this for initialization
	void Start ()
	{
	    SplashImage.color = new Color(SplashImage.color.r, SplashImage.color.g, SplashImage.color.b, 0);
	}
	
	// Update is called once per frame
	void Update ()
	{
        // so we can skip
	    if (Input.anyKeyDown)
	        _internalTimer += SplashTimer;

	    if (_internalTimer >= SplashTimer)
	    {
	        if (_splashOut >= SplashOutTimer)
	        {
                var fade = new FadeTransition()
                {
                    nextScene = "Start",
                    duration = 0.5f,
                    fadeToColor = Color.black,
                    fadedDelay = 0.0f
                };
                TransitionKit.instance.transitionWithDelegate(fade);
            }
	        SplashImage.color = Color.Lerp(SplashImage.color, Color.black, 0.09f);
	        _splashOut += Time.deltaTime;
	    }

        // fade the image here
        else
            SplashImage.color = Color.Lerp(SplashImage.color, Color.white, 0.02f);

	    _internalTimer += Time.deltaTime;
	}
}
