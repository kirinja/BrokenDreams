using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Pushable : MonoBehaviour
{
    private const float GroundCheckDistance = 0.2f;
    private const float PushSpeed = 15f;

    private AbilityColors _abilityColor;
    private bool _isPushed;
    private Material _material;
    private Vector3 _startPosition;
    private float _totalPushLength;
    private Vector3 _velocity;
    private AudioSource _slideSource, _collideSource;
    private bool _inAir, _initiated;

    public LayerMask CollisionMask;


    // Use this for initialization
    private void Start()
    {
        _material = GetComponent<Renderer>().sharedMaterial;
        _abilityColor = Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors;
        _material.color = _abilityColor.PushColor;
        _velocity = Vector3.zero;
        _totalPushLength = 0f;
        _isPushed = false;
        var sources = GetComponents<AudioSource>();
        _slideSource = sources[0];
        _collideSource = sources[1];
    }


    // UpdateAbility is called once per frame
    private void FixedUpdate()
    {
        if (_isPushed)
        {
            var previousPosition = transform.position;

            transform.position += _velocity * Time.deltaTime;

            var pushedLength = CalculateDistancePushed();

            var didHit = false;
            var hits = Physics.OverlapBox(transform.position, GetComponent<Collider>().bounds.extents,
                transform.rotation, CollisionMask);

            foreach (var collider in hits)
            {
                if (collider.gameObject == gameObject) continue;

                if (collider.CompareTag("Movable Object"))
                {
                    collider.transform.Translate(0.1f * _velocity.normalized);
                    collider.GetComponent<Pushable>().Push(_velocity.normalized, _totalPushLength - pushedLength);
                    _isPushed = false;
                    _collideSource.Play();
                }
                else if (collider.CompareTag("Enemy"))
                {
                    var damageable = collider.GetComponent<Enemy>();
                    if (damageable)
                        damageable.Damage(9999);
                }
                else
                {
                    didHit = true;
                    _collideSource.Play();
                }
            }

            if (didHit)
            {
                transform.position = previousPosition;
                _isPushed = false;
            }
            else if (pushedLength >= _totalPushLength)
            {
                transform.position = new Vector3(_startPosition.x + _totalPushLength * Mathf.Sign(_velocity.x),
                    transform.position.y, transform.position.z);
                _isPushed = false;
            }

            
        }

        if (!CheckGround())
        {
            _isPushed = false;
        }
        HandleGravity();

        if (!_isPushed)
        {
            _slideSource.Stop();
        }

        GetComponent<Rigidbody>().position = transform.position;
    }


    private bool CheckGround()
    {
        var col = GetComponent<Collider>();
        var isGrounded = false;

        var collisions = Physics.RaycastAll(
            new Vector3(transform.position.x - col.bounds.extents.x,
                transform.position.y - col.bounds.extents.y, transform.position.z), Vector3.down,
            GroundCheckDistance, CollisionMask);
        foreach (var rayHit in collisions)
        {
            if (rayHit.transform.gameObject == gameObject || rayHit.transform.CompareTag("Enemy")) continue;

            isGrounded = true;
        }

        collisions = Physics.RaycastAll(
            new Vector3(transform.position.x + col.bounds.extents.x,
                transform.position.y - col.bounds.extents.y, transform.position.z), Vector3.down,
            GroundCheckDistance, CollisionMask);
        foreach (var rayHit in collisions)
        {
            if (rayHit.transform.gameObject == gameObject || rayHit.transform.CompareTag("Enemy")) continue;

            isGrounded = true;
        }

        if (isGrounded)
        {
            if (_inAir && !_collideSource.isPlaying && _initiated)
            {
                _collideSource.Play();
            }

            _initiated = true;
            _inAir = false;
        }
        else
        {
            _inAir = true;
        }

        return isGrounded;
    }


    private void HandleGravity()
    {
        var gravity = Physics.gravity;
        _velocity += gravity * Time.deltaTime;
        var previousPosition = transform.position;

        transform.position += new Vector3(0f, _velocity.y * Time.deltaTime, 0f);

        var didHit = false;
        var hits = Physics.OverlapBox(transform.position, GetComponent<Collider>().bounds.extents, transform.rotation,
            CollisionMask);
        var enemies = new List<Enemy>();

        foreach (var collider in hits)
        {
            if (collider.gameObject == gameObject) continue;

            if (collider.CompareTag("Enemy"))
            {
                var damageable = collider.GetComponent<Enemy>();
                if (damageable)
                    enemies.Add(damageable);
            }
            else
            {
                didHit = true;
            }
        }

        if (didHit)
        {
            
            transform.position = previousPosition;
            _velocity = new Vector3(_velocity.x, 0f, _velocity.z);
        }
        else
        {
            
            foreach (var enemy in enemies)
            {
                enemy.Damage(9999);
            }
        }
    }


    private float CalculateDistancePushed()
    {
        if (_velocity.x > 0f)
            return transform.position.x - _startPosition.x;

        return _startPosition.x - transform.position.x;
    }


    private void UpdateColor()
    {
        // this snippet only run when in the editor
#if UNITY_EDITOR
        if (Application.isEditor && !Application.isPlaying)
            _material.color = _abilityColor.PushColor;
#endif
    }


#if UNITY_EDITOR
    private void OnEnable()
    {
        EditorApplication.update += UpdateColor;
    }
#endif



    public void Push(Vector3 direction, float length)
    {
        if (_isPushed || _inAir) return;

        var localVel = direction;

        _totalPushLength = length;

        var pushDirection = new Vector3(localVel.x, 0f, 0f).normalized;
        var horizontalVvelocity = pushDirection * PushSpeed;
        _velocity = new Vector3(horizontalVvelocity.x, _velocity.y, 0f);
        _isPushed = true;
        _startPosition = transform.position;

        _slideSource.Play();
    }
}