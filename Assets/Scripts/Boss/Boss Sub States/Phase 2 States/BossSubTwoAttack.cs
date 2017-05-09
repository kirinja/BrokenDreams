using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class BossSubTwoAttack : IBossSubState
{

    private BossBehaviour _bossData;
    private GameObject _head;
    private float _timer;

    private int _projCounter = 0;
    private float _projTimer;

    private int _phaseStartHp;
    
    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        _phaseStartHp = _bossData.HP;
        _timer = new System.Random().Next((int)_bossData.MinStateSwitch, (int)_bossData.MaxStateSwitch); // HACK

        _projTimer = _bossData.TimeBetweenShots;

        _head = GameObject.Find("Head");

        _bossData.PlayPhaseTwoAttackSound();
    }

    public IBossSubState Execute()
    {
        if (_bossData.HP < _phaseStartHp)
        {
            // boss was damaged, go to defend state
            return new BossSubTwoDefend();
        }
        ShowHead();

        if (CanShoot())
            ShootProjectiles();

        UpdateTimers();

        if (!(_timer <= 0.0f)) return null;

        //if (Random.value <= 0.5f)
        //return new BossSubTwoDefend();

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
        var g = Object.Instantiate(_bossData.Projectiles, _head.transform.position , Quaternion.identity);
        g.transform.position = new Vector3(g.transform.position.x - 3.5f, g.transform.position.y, -1); // HACK MAGIC VALUES

        _projCounter++;
        _projTimer = _bossData.TimeBetweenShots;

        _bossData.PlayProjSound();
    }

    private void ShowHead()
    {
        // lerp this shit
        _head.transform.position = Vector3.Lerp(_head.transform.position, _bossData.Phase2AttackPos.position, 0.25f);
        _head.transform.localScale = Vector3.Lerp(_head.transform.localScale, new Vector3(1, 1, 1), 0.25f);
        //_head.transform.position = _bossData.Phase2AttackPos.position;
        //_head.GetComponent<Renderer>().enabled = true;
        //if (Mathf.Abs(_head.transform.position.y - _bossData.Phase2AttackPos.position.y) >= 0.5f)
        if (Math.Abs(_head.transform.position.y - _bossData.Phase2AttackPos.position.y) < 0.5f)
        {
            var cols = _head.GetComponents<Collider>();
            foreach (var col in cols)
                col.enabled = true;
        }

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
