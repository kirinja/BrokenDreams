using Prime31.TransitionKit;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelGoal : MonoBehaviour
{
    public AudioClip goalSound;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Activate();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && ( Input.GetAxisRaw("Vertical") >= 0.5f))
        {
            //GameManager.Instance.Play(goalSound);
            //GameManager.Instance.NextLevelToLoad = "Hub";
            //var doorway = new DoorwayTransition()
            //{
            //    nextScene = "LoadingScreen",
            //    duration = 1.0f,
            //    perspective = 1.5f,
            //    depth = 3f,
            //    runEffectInReverse = true
            //};

            //TransitionKit.instance.transitionWithDelegate(doorway);
            
            Activate();
        }
    }


    private void Activate()
    {
        var gameManager = GameManager.Instance;
        var firstTime = gameManager.BeatLevel(SceneManager.GetActiveScene().name);
        gameManager.SaveToMemory();
        gameManager.SaveToFiles();
        gameManager.UseCheckPoint = false;

        /*var fishEye = new FishEyeTransition
        {
            nextScene = "Hub",
            duration = 2.0f,
            size = 0.2f,
            zoom = 100.0f,
            colorSeparation = 0.1f
        };
        TransitionKit.instance.transitionWithDelegate(fishEye);/**/
        gameManager.Play(goalSound);
        gameManager.ReturnToHub(SceneManager.GetActiveScene().name, firstTime);
    }
}