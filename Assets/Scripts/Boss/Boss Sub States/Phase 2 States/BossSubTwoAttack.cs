using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossSubTwoAttack : IBossSubState
{

    private BossBehaviour _bossData;
    private GameObject _head;
    private float _timer;

    private int _projCounter = 0;
    private const float TimeBetweenShots = 0.5f;
    private float _projTimer;

    
    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        _timer = new System.Random().Next((int)_bossData.MinStateSwitch, (int)_bossData.MaxStateSwitch); // HACK

        _projTimer = TimeBetweenShots;

        _head = GameObject.Find("Head");
        
    }

    public IBossSubState Execute()
    {
        _head.transform.position = _bossData.Phase2AttackPos.position;
        _head.GetComponent<Renderer>().enabled = true;
        var cols = _head.GetComponents<Collider>();
        foreach (var col in cols)
            col.enabled = true;

        

        if (_projCounter < _bossData.Phase3Projectiles && _projTimer <= 0.0f)
        {
            // spawn projectiles and launch them at the player
            var g = Object.Instantiate(_bossData.Projectiles, _head.transform.position, Quaternion.identity);
            g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, -1);

            _projCounter++;
            _projTimer = TimeBetweenShots;

            _bossData.PlayBossProjSound();
        }

        
        _projTimer -= Time.deltaTime;
        _timer -= Time.deltaTime;

        if (!(_timer <= 0.0f)) return null;
        var r = Random.value;
        if (r <= 0.5f)
            return new BossSubTwoDefend();

        return new BossSubTwoIdle();
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
