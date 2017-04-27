using UnityEngine;


public class CheckPointHandler : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
        var gm = GameManager.Instance;
        if (gm.UseCheckPoint)
            gm.LoadCheckPointData();

        var player = GameObject.FindGameObjectWithTag("Player");
        if (!player) return;

        var controller = player.GetComponent<Controller3D>();
        if (controller)
            controller.Spawn();
    }
}