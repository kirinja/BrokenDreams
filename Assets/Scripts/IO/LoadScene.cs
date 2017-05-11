using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    private bool _loading;
    public Text LoadingText;

    private AsyncOperation operation;
    public LoadSceneMode loadSceneMode = LoadSceneMode.Single;
    // Use this for initialization
    void Start ()
	{
	    var name = GameManager.Instance.NextLevelToLoad;
	    StartCoroutine(LoadLevelAsync(name));

	    //_loading = true;
	    ////LoadingScreenManager.LoadScene(name);
	    //LoadingScreenManager.sceneToLoad = name;
        //GameManager.Instance.GetComponent<LoadingScreenManager>().Start();
	}

    void StartLoadScene(string name)
    {
        Application.backgroundLoadingPriority = ThreadPriority.High;
        SceneManager.LoadScene(name);
    }

    // Update is called once per frame
    void Update()
    {
        //if (_loading)
        //{
        //    //Debug.Log("Loading level");
        //    LoadingText.color = new Color(LoadingText.color.r, LoadingText.color.g, LoadingText.color.b, Mathf.PingPong(Time.time, 1));

        //}
    }

    IEnumerator AsynchronousLoad(string scene)
    {
        yield return null;

        AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            // [0, 0.9] > [0, 1]
            float progress = Mathf.Clamp01(ao.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            // Loading completed
            if (ao.progress == 0.9f)
            {
                Debug.Log("Press a key to start");
                if (Input.anyKeyDown)
                    ao.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    IEnumerator LoadLevelAsync(string name)
    {
        _loading = true;
        yield return new WaitForSeconds(1.5f);

        yield return null;
        
        AsyncOperation async = SceneManager.LoadSceneAsync(name);
        async.allowSceneActivation = false;
        

        while (!async.isDone)
        {
            // [0, 0.9] > [0, 1]
            float progress = Mathf.Clamp01(async.progress / 0.9f);
            //Debug.Log("Loading progress: " + (progress * 100) + "%");
            LoadingText.color = new Color(LoadingText.color.r, LoadingText.color.g, LoadingText.color.b, Mathf.PingPong(Time.time, 1));

            // Loading completed
            if (Math.Abs(async.progress - 0.9f) < 0.01f)
            {
                //Debug.Log("Press a key to start");
                //if (Input.anyKey)
                    async.allowSceneActivation = true;
            }
            
            yield return null;
        }

        yield return null;
    }

    private bool DoneLoading()
    {
        return (loadSceneMode == LoadSceneMode.Additive && operation.isDone) || (loadSceneMode == LoadSceneMode.Single && operation.progress >= 0.9f);
    }
}
