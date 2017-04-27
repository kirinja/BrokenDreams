using UnityEngine;


public class Loader : MonoBehaviour
{
    public GameObject GameManagerPrefab; //GameManager prefab to instantiate.


    private void Awake()
    {
        if (GameManager.Instance == null)
            Instantiate(GameManagerPrefab);
    }
}