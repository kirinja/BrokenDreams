﻿using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Attack")]
class AttackAbility : Ability
{
    public float AttackRange = 1f;

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
        var offset = (upwards ? new Vector3(0f, AttackRange / 2f, 0f) : new Vector3((AttackRange / 2f) * controller.Forward.normalized.x, 0f, 0f)) + baseOffset;

        var particle = controller.transform.Find("Hit");
        particle.localEulerAngles = new Vector3(upwards ? -45f : 0f, 0f, 0f);
        particle.Find("Slash").localScale = new Vector3(0.4f, 0.4f, 0.4f);
        particle.localPosition = new Vector3(0f, upwards ? 0.5f : 0f, upwards ? 0f : 0.5f);

        foreach (var p in controller.transform.Find("Hit").GetComponentsInChildren<ParticleSystem>())
        {
            p.Play();
        }

        //particle.Find("Slash").GetComponent<ParticleSystem>().Play();
        controller.Animator.SetTrigger("Attack");

        var hitbox = Instantiate(HitboxPrefab,
            controller.transform.position + offset, Quaternion.identity);
        hitbox.GetComponent<PlayerHitbox>().Width = upwards ? 0.8f : AttackRange;
        hitbox.GetComponent<PlayerHitbox>().Height = upwards ? AttackRange : 0.8f;
        hitbox.transform.localScale = Vector3.one * AttackRange * 2;
        hitbox.transform.SetParent(controller.transform);

        return new CharacterStateSwitch3D();
    }
}