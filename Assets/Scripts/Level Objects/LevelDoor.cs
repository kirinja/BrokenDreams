using UnityEngine;


public class LevelDoor : MonoBehaviour
{
    public bool Inverted;
    public string Scene;


    // Use this for initialization
    private void Start()
    {
        gameObject.SetActive(Inverted
            ? !GameManager.Instance.LevelAvailable(Scene)
            : GameManager.Instance.LevelAvailable(Scene));
    }


    // Update is called once per frame
    private void Update()
    {
    }
}