using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Attack")]
class AttackAbility : Ability
{
    [SerializeField]
    private float AttackRadius;

    private void OnEnable()
    {
        Color = (Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors).AttackColor;
    }

    public override CharacterStateSwitch3D Use(Controller3D controller)
    {
        timeLeft = Cooldown;

        var upwards = controller.MovementInput.y > 0.5f;
        const float offsetLength = 0.5f;
        var offset = upwards ? new Vector3(0f, offsetLength, 0f) : new Vector3(offsetLength, 0f, 0f);

        var particle = controller.transform.Find("Hit");
        particle.localEulerAngles = new Vector3(upwards ? -45f : 0f, 0f, 0f);
        particle.Find("Slash").GetComponent<ParticleSystem>().Play();
        controller.Animator.SetTrigger("Attack");

        var hits =
            Physics.OverlapSphere(
                controller.transform.position + offset, AttackRadius);

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