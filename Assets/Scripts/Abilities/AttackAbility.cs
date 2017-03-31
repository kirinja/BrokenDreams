using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Attack")]
class AttackAbility : Ability
{
    private void OnEnable()
    {
        Color = (Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors).AttackColor;
    }

    public override CharacterStateSwitch3D Use(Controller3D controller)
    {
        timeLeft = Cooldown;

		controller.transform.Find("Hit").Find("Slash").GetComponent<ParticleSystem>().Play();


		controller.animator.SetTrigger ("Attack");


        var hits =
            Physics.OverlapSphere(
                controller.transform.position + controller.transform.TransformDirection(0f, 0f, 0.5f), 1.5f);

        foreach (var gameObject in hits)
        {
            var hitObject = gameObject.GetComponent<Attackable>();
            if (hitObject)
            {
                hitObject.Damage();
            }
        }

        return new CharacterStateSwitch3D();
    }
}