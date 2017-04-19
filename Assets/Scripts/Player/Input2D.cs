using UnityEngine;

[RequireComponent(typeof(Controller3D))]
public class Input2D : MonoBehaviour
{
    public float LockedZPosition = -1;

    private Controller3D controller;
    private bool[] abilityInputs;

    private void Awake()
    {
        abilityInputs = new bool[4];
    }

    private void Start()
    {
        controller = GetComponent<Controller3D>();
    }

    private void Update()
    {
        var gm = GameManager.Get();
        if (gm && gm.Paused) return;
        for (var i = 0; i < abilityInputs.Length; ++i)
        {
            abilityInputs[i] = Input.GetButtonDown("Use Ability " + (i + 1).ToString()) || abilityInputs[i];
        }
    }

    private void FixedUpdate()
    {
        var gm = GameManager.Get();
        if (gm && gm.Paused) return;
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        controller.HandleMovement(abilityInputs, input);
        controller.SetPosition(new Vector3(controller.transform.position.x, controller.transform.position.y,
            LockedZPosition));

        for (var i = 0; i < abilityInputs.Length; ++i)
        {
            abilityInputs[i] = false;
        }
    }
}