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
        Debug.Log("Using Attack");
        timeLeft = Cooldown;

        var hits =
            Physics.OverlapSphere(
                controller.transform.position + controller.transform.TransformDirection(0f, 0f, 0.5f), 1.5f);

        foreach (var gameObject in hits)
        {
            if (gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Found enemy");
                var enemy = gameObject.GetComponent<Enemy>();
                if (enemy)
                {
                    enemy.Damage();
                }
            }
        }

        return new CharacterStateSwitch3D();
    }
}