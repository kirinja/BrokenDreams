using UnityEditor;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    public LayerMask CollisionMask;

    private AbilityColors abilityColor;
    private bool isPushed;
    private Material material;
    private Vector3 startPosition;
    private float totalPushLength;
    private Vector3 velocity;
    private const float GroundCheckDistance = 0.1f;
    private const float PushSpeed = 15f;

    // Use this for initialization
    private void Start()
    {
        material = GetComponent<Renderer>().sharedMaterial;
        abilityColor = Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors;
        material.color = abilityColor.PushColor;
        velocity = Vector3.zero;
        totalPushLength = 0f;
        isPushed = false;
    }

    // UpdateAbility is called once per frame
    private void Update()
    {
        if (isPushed)
        {
            var previousPosition = transform.position;

            transform.position += velocity * Time.deltaTime;

            var pushedLength = CalculateDistancePushed();

            var didHit = false;
            var hits = Physics.OverlapBox(transform.position, GetComponent<Collider>().bounds.extents, transform.rotation, CollisionMask);

            foreach (var collider in hits)
            {
                if (collider.gameObject == gameObject) continue;

                if (collider.CompareTag("Movable Object"))
                {
                    collider.transform.Translate(0.1f * velocity.normalized);
                    collider.GetComponent<Pushable>().Push(velocity.normalized, totalPushLength - pushedLength);
                    isPushed = false;
                }
                else if (collider.CompareTag("Enemy"))
                {
                    var damageable = collider.GetComponent<Attackable>();
                    if (damageable)
                    {
                        damageable.Damage();
                    }
                }
                else
                {
                    didHit = true;
                }
            }
            if (didHit)
            {
                transform.position = previousPosition;
                isPushed = false;
            }
            else if (pushedLength >= totalPushLength)
            {
                transform.position = new Vector3(startPosition.x + totalPushLength * Mathf.Sign(velocity.x),
                    transform.position.y, transform.position.z);
                isPushed = false;
            }
            
        }

        if (!CheckGround())
        {
            isPushed = false;
        }
        HandleGravity();

        GetComponent<Rigidbody>().position = transform.position;
    }

    private bool CheckGround()
    {
        var collider = GetComponent<Collider>();
        var isGrounded = false;
        RaycastHit hitData;
        isGrounded = (Physics.Raycast(
                         new Vector3(transform.position.x - collider.bounds.extents.x,
                             transform.position.y - collider.bounds.extents.y, transform.position.z), Vector3.down, out hitData,
                         GroundCheckDistance, CollisionMask) && hitData.transform.gameObject != gameObject);
        isGrounded = (Physics.Raycast(
                         new Vector3(transform.position.x + collider.bounds.extents.x,
                             transform.position.y - collider.bounds.extents.y, transform.position.z), Vector3.down, out hitData,
                         GroundCheckDistance, CollisionMask) && hitData.transform.gameObject != gameObject) || isGrounded;

        return isGrounded;
    }

    private void HandleGravity()
    {
        var gravity = Physics.gravity;
        velocity += gravity * Time.deltaTime;
        var previousPosition = transform.position;

        transform.position += new Vector3(0f, velocity.y * Time.deltaTime, 0f);

        var didHit = false;
        var hits = Physics.OverlapBox(transform.position, GetComponent<Collider>().bounds.extents, transform.rotation, CollisionMask);

        foreach (var collider in hits)
        {
            if (collider.gameObject == gameObject) continue;

            if (collider.CompareTag("Enemy"))
            {
                var damageable = collider.GetComponent<Attackable>();
                if (damageable)
                {
                    damageable.Damage();
                }
            }
            else
            {
                didHit = true;
            }
            
            break;
        }
        if (didHit)
        {
            transform.position = previousPosition;
            velocity = new Vector3(velocity.x, 0f, velocity.z);
        }
        /*else
        {
            transform.position = previousPosition;
            isPushed = false;
        }*/
    }

    private float CalculateDistancePushed()
    {
        if (velocity.x > 0f)
        {
            return transform.position.x - startPosition.x;
        }
        return startPosition.x - transform.position.x;
    }

    private void UpdateColor()
    {
        // this snippet only run when in the editor
#if UNITY_EDITOR
        if (Application.isEditor && !Application.isPlaying)
            material.color = abilityColor.PushColor;
#endif
    }

    private void OnEnable()
    {
        EditorApplication.update += UpdateColor;
    }

    public void Push(Vector3 direction, float length)
    {
        if (isPushed) return;

        var localVel = direction;

        totalPushLength = length;

        var pushDirection = new Vector3(localVel.x, 0f, 0f).normalized;
        var horizontalVvelocity = pushDirection * PushSpeed;
        velocity = new Vector3(horizontalVvelocity.x, velocity.y, 0f);
        isPushed = true;
        startPosition = transform.position;
    }
}