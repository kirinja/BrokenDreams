using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossSubTwoAttack : IBossSubState
{

    private BossBehaviour _bossData;
    private GameObject _head;
    private float _timer;

    private int _projCounter = 0;
    private float _projTimer;

    
    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        _timer = new System.Random().Next((int)_bossData.MinStateSwitch, (int)_bossData.MaxStateSwitch); // HACK

        _projTimer = _bossData.TimeBetweenShots;

        _head = GameObject.Find("Head");
    }

    public IBossSubState Execute()
    {
        ShowHead();

        if (CanShoot())
            ShootProjectiles();

        UpdateTimers();

        if (!(_timer <= 0.0f)) return null;

        if (Random.value <= 0.5f)
            return new BossSubTwoDefend();

        return new BossSubTwoIdle();
    }

    private bool CanShoot()
    {
        return _projCounter < _bossData.Phase2Projectiles && _projTimer <= 0.0f;
    }

    private void UpdateTimers()
    {
        _projTimer -= Time.deltaTime;
        _timer -= Time.deltaTime;
    }

    private void ShootProjectiles()
    {
        // spawn projectiles and launch them at the player
        var g = Object.Instantiate(_bossData.Projectiles, _head.transform.position, Quaternion.identity);
        g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, -1);

        _projCounter++;
        _projTimer = _bossData.TimeBetweenShots;

        _bossData.PlayBossProjSound();
    }

    private void ShowHead()
    {
        _head.transform.position = _bossData.Phase2AttackPos.position;
        _head.GetComponent<Renderer>().enabled = true;
        var cols = _head.GetComponents<Collider>();
        foreach (var col in cols)
            col.enabled = true;
    }

    public void Exit()
    {
    }

    public bool Alive()
    {
        return _bossData.HP <= _bossData.damageSwitchPhase2;
    }

    public void TakeDamage(int value)
    {
        _bossData.HP -= value;
    }
}
