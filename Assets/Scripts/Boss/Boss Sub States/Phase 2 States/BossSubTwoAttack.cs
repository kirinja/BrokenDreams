using System.Linq;
using UnityEngine;

public class BossSubTwoAttack : IBossSubState
{

    private BossBehaviour _bossData;
    private GameObject _head;
    private bool _spawned;
    private float _timer;
    private float _projDelay = 0.5f; // TODO implement phase 2 projectiles

    public void Enter(BossBehaviour data)
    {
        _bossData = data;
        _timer = _bossData.StateSwitchTimer;
        

        _head = GameObject.Find("Head");
    }

    public IBossSubState Execute()
    {
        // behaviour for spawning enemies
        Debug.Log("Phase 2 Attack State");
        // we cant spawn enemies  like this, it needs to happen once and then move back to idle, otherwise we're gonna spawn enemies every frame for X amount of time
        //Debug.Log("Spawn enemy 1 at random locations");
        // use a _timer or something to determine when we should switch state

        _head.transform.position = _bossData.Phase2AttackPos.position;

        //cant set active since we cant find it in the hierarchy afterwards. we should disable the colliders and renderer
        //_head.SetActive(true);
        _head.GetComponent<Renderer>().enabled = true;
        var cols = _head.GetComponents<Collider>();
        foreach (var col in cols)
            col.enabled = true;

        if (!_spawned)
        {
            var spawnPoints = GameObject.FindGameObjectsWithTag("Wall");

            var rand = new System.Random();

            // setup the platform ID's
            int[] arr = new int[spawnPoints.Length];
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                arr[i] = i;
            }

            // TODO this is broken, we need an enemy2 prefab but an enemy2 needs target to patrol between, which isnt included in the prefab
            // TODO enemy2 also need a projectile which isnt included in the prefab
            // TODO need to look over enemy2 and make it self-contained, or at least make it find the data it needs to work (cant manually assign when instanciating
            for (int i = 0; i < _bossData.Phase2Spawn; i++)
            {
                var index = rand.Next(0, arr.Length);
                var v = arr[index];
                Debug.Log("Spawn at platform ID " + v);
                GameObject.Instantiate(_bossData.Enemy2, spawnPoints[v].transform.position + new Vector3(0, spawnPoints[v].transform.localScale.y / 2 + _bossData.Enemy1.transform.localScale.y / 2, -1), Quaternion.identity);
                arr = arr.Where(val => val != v).ToArray();
                
            }
            _spawned = true;
        }

        _timer -= Time.deltaTime;
        return _timer <= 0.0f ? new BossSubTwoDefend() : null;
        //throw new System.NotImplementedException();
    }

    public void Exit()
    {
        //throw new System.NotImplementedException();
        _spawned = false;
    }

    public bool Alive()
    {
        return _bossData.HP <= _bossData.phase2;
    }

    public void TakeDamage(int value)
    {
        _bossData.HP -= value;
    }
}
