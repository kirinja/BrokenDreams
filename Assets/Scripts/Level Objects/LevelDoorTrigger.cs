using Prime31.TransitionKit;
using UnityEngine;

public class LevelDoorTrigger : MonoBehaviour
{
    public string Scene;
    public bool RequiresInput = true;
    public AudioClip enterSound;
    

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && (!RequiresInput || Input.GetAxisRaw("Vertical") >= 0.5f) && GameManager.Instance.LevelAvailable(Scene))
        {
            //var fishEye = new FishEyeTransition()
            //{
            //    nextScene = Scene,
            //    duration = 2.0f,
            //    size = 0.2f,
            //    zoom = 100.0f,
            //    colorSeparation = 0.1f
            //};
            //TransitionKit.instance.transitionWithDelegate(fishEye);
            GameManager.Instance.Play(enterSound);
            GameManager.Instance.NextLevelToLoad = Scene;
            var doorway = new DoorwayTransition()
            {
                nextScene = "LoadingScreen",
                duration = 1.0f,
                perspective = 1.5f,
                depth = 3f,
                runEffectInReverse = true
            };
            
            TransitionKit.instance.transitionWithDelegate(doorway);
            //SceneManager.LoadScene(Scene);
        }
    }
}
