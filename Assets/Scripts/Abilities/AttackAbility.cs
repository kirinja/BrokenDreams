using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Attack")]
class AttackAbility : Ability
{
    public float AttackRadius;

    public GameObject HitboxPrefab;

    
    private void OnEnable()
    {
        Color = (Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors).AttackColor;
    }


    public override CharacterStateSwitch3D Use(Controller3D controller)
    {
        timeLeft = Cooldown;

        var baseOffset = new Vector3(0f, 0.4f, 0f);
        var upwards = controller.MovementInput.y > 0.5f;
        var offset = (upwards ? new Vector3(0f, AttackRadius, 0f) : new Vector3(AttackRadius * controller.Forward.x, 0f, 0f)) + baseOffset;

        var particle = controller.transform.Find("Hit");
        particle.localEulerAngles = new Vector3(upwards ? -45f : 0f, 0f, 0f);
        particle.Find("Slash").localScale = new Vector3(0.4f, 0.4f, 0.4f);
        particle.localPosition = new Vector3(0f, upwards ? 0.5f : 0f, upwards ? 0f : 0.5f);
        particle.Find("Slash").GetComponent<ParticleSystem>().Play();
        controller.Animator.SetTrigger("Attack");

        var hitbox = Instantiate(HitboxPrefab,
            controller.transform.position + offset, Quaternion.identity);
        hitbox.transform.localScale = Vector3.one * AttackRadius * 2;
        hitbox.transform.SetParent(controller.transform);

        /*var hits =
            Physics.OverlapSphere(
                controller.transform.position + offset, AttackRadius);

        

        foreach (var gameObject in hits)
        {
            var hitObject = gameObject.GetComponent<Attackable>();
            if (hitObject)
            {
                hitObject.Damage();
            }
        }/**/

        return new CharacterStateSwitch3D();
    }
}