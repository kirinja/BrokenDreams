using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public float MinimumTime = 1.0f;

    // Use this for initialization
    void Start ()
	{
	    var name2 = GameManager.Instance.NextLevelToLoad;
	    //StartCoroutine(LoadLevelAsync(name));
        LoadLevelTest(name2);
	}


    void LoadLevelTest(string levelName)
    {
        GameManager.Instance.GetComponent<ScreenManager>().LoadLevel(levelName);
    }
    
    IEnumerator LoadLevelAsync(string name)
    {
        yield return new WaitForSeconds(MinimumTime);
        //yield return GameManager.Instance.FadeIn2();

        //GameManager.Instance.FadeIn();
        //yield return null;
        
        AsyncOperation async = SceneManager.LoadSceneAsync(name);
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
            }
            
            yield return null;
        }
        

        //yield return GameManager.Instance.FadeOut2();
        
        yield return null;
    }
}
