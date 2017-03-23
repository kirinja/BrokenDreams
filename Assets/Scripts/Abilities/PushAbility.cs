using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Push")]
class PushAbility : Ability
{
    private void OnEnable()
    {
        Color = (Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors).PushColor;
    }

    public override CharacterStateSwitch3D Use(Controller3D controller)
    {
        Debug.Log("Using Push");
        timeLeft = Cooldown;

        var hits =
            Physics.OverlapSphere(
                controller.transform.position + controller.transform.TransformDirection(0f, 0f, 0.5f), 1.5f);

        foreach (var gameObject in hits)
        {
            Debug.Log("Found pushable");
            var pushable = gameObject.GetComponent<Pushable>();
            if (pushable)
            {
                pushable.Push(controller.transform.forward);
            }
        }

        return new CharacterStateSwitch3D();
    }
}