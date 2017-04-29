using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    public float SplashTimer = 2.0f;
    private float _internalTimer = 0;
    public Image SplashImage;

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
            SceneManager.LoadScene("Start");

        // fade the image here
	    SplashImage.color = Color.Lerp(SplashImage.color, Color.white, 0.02f);

	    _internalTimer += Time.deltaTime;
	}
}
