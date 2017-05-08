using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    private bool _loading;
    public Text LoadingText;
	// Use this for initialization
	void Start ()
	{
	    var name = GameManager.Instance.NextLevelToLoad;
	    StartCoroutine(LoadLevelAsync(name));
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (_loading)
        {
            Debug.Log("Loading level");
            LoadingText.color = new Color(LoadingText.color.r, LoadingText.color.g, LoadingText.color.b, Mathf.PingPong(Time.time, 1));

        }
	}

    IEnumerator LoadLevelAsync(string name)
    {
        _loading = true;
        yield return new WaitForSeconds(3.0f);
        
        AsyncOperation async = SceneManager.LoadSceneAsync(name);

        

        while (!async.isDone)
        {
            yield return null;
        }
    }
}
