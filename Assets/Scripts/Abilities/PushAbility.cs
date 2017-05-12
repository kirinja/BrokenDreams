using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Push")]
class PushAbility : Ability
{
    public float PushRange = 0.5f;
    public float PushLength = 5f;

    private float pushSpeed;
    private Controller3D controller;
    private bool active;

    private void OnEnable()
    {
        Color = (Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors).PushColor;
    }

    public override CharacterStateSwitch3D Use(Controller3D controller)
    {
        active = true;
        this.controller = controller;
        pushSpeed = controller.Attributes.MaxSpeed;
        timeLeft = Cooldown;



		controller.Animator.SetTrigger("Push");

		//controller.transform.Find("Push").Find("SpeedLines").GetComponent<ParticleSystem>().Play();
		//controller.transform.Find("Push").Find("Magic").GetComponent<ParticleSystem>().Play();

        foreach (var p in controller.transform.Find("Push").GetComponentsInChildren<ParticleSystem>())
        {
            p.Play();
        }



        RaycastHit hitInfo;
        if (Physics.Raycast(controller.transform.position,
            controller.Forward, out hitInfo, PushRange + controller.GetComponent<Collider>().bounds.extents.z))
        {
            var pushable = hitInfo.transform.GetComponent<Pushable>();
            if (pushable)
            {
                pushable.Push(controller.Forward, PushLength);
            }
        }

        return new CharacterStateSwitch3D();
    }

    public override void UpdateAbility()
    {
        base.UpdateAbility();
    }
}