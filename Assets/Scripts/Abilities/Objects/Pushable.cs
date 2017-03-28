using UnityEditor;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    public float PushSpeed = 5f;
    public LayerMask CollisionMask;

    private AbilityColors abilityColor;
    private bool isPushed;
    private Material material;
    private Axis pushAxis;
    private Vector3 startPosition;
    private float totalPushLength;
    private Vector3 velocity;

    // Use this for initialization
    private void Start()
    {
        material = GetComponent<Renderer>().sharedMaterial;
        abilityColor = Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors;
        material.color = abilityColor.PushColor;
        velocity = Vector3.zero;
        totalPushLength = 0f;
        isPushed = false;
        pushAxis = Axis.NONE;
    }

    // UpdateTime is called once per frame
    private void Update()
    {
        if (isPushed)
        {
            string debugString = "";
            debugString += "Collision before moving: " +
                           Physics.OverlapBox(transform.position, GetComponent<Collider>().bounds.extents,
                               transform.rotation, CollisionMask).Length;

            var previousPosition = transform.position;

            transform.position += velocity * Time.deltaTime;

            var pushedLength = CalculateDistancePushed();

            var didHit = false;
            var hits = Physics.OverlapBox(transform.position, GetComponent<Collider>().bounds.extents, transform.rotation, CollisionMask);

            debugString += " Collision after moving: " + hits.Length;
            Debug.Log(debugString);

            foreach (var collider in hits)
            {
                if (collider.gameObject == gameObject) continue;

                if (collider.CompareTag("Movable Object"))
                {
                    collider.GetComponent<Pushable>().Push(velocity.normalized);
                }
                else
                {
                    Debug.Log("Box colliding with something...");
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
                switch (pushAxis)
                {
                    case Axis.X:
                        transform.position = new Vector3(startPosition.x + totalPushLength * Mathf.Sign(velocity.x),
                            transform.position.y, transform.position.z);
                        break;
                    case Axis.Z:
                        transform.position = new Vector3(startPosition.x, transform.position.y,
                            transform.position.z + totalPushLength * Mathf.Sign(velocity.z));
                        break;
                }
                isPushed = false;
            }
        }

        HandleGravity();

        GetComponent<Rigidbody>().position = transform.position;
    }

    private void HandleGravity()
    {
        string debugString = "";
        debugString += "Collision before gravity: " +
                       Physics.OverlapBox(transform.position, GetComponent<Collider>().bounds.extents,
                           transform.rotation, CollisionMask).Length;

        var gravity = Physics.gravity;
        velocity += gravity * Time.deltaTime;
        var previousPosition = transform.position;

        transform.position += new Vector3(0f, velocity.y * Time.deltaTime, 0f);

        var didHit = false;
        var hits = Physics.OverlapBox(transform.position, GetComponent<Collider>().bounds.extents, transform.rotation, CollisionMask);

        debugString += " Collision after gravity: " + hits.Length;
        Debug.Log(debugString);

        foreach (var collider in hits)
        {
            if (collider.gameObject == gameObject) continue;

            didHit = true;
            break;
        }
        if (didHit)
        {
            transform.position = previousPosition;
            velocity = new Vector3(velocity.x, 0f, velocity.z);
        }
    }

    private float CalculateDistancePushed()
    {
        switch (pushAxis)
        {
            case Axis.X:
                if (velocity.x > 0f)
                {
                    return transform.position.x - startPosition.x;
                }
                return startPosition.x - transform.position.x;
            case Axis.Z:
                if (velocity.z > 0f)
                {
                    return transform.position.z - startPosition.z;
                }
                return startPosition.z - transform.position.z;
            default:
                return 0f;
        }
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

    public void Push(Vector3 direction)
    {
        if (isPushed) return;

        var localVel = transform.InverseTransformDirection(direction);
        pushAxis = Mathf.Abs(localVel.x) > Mathf.Abs(localVel.z) ? Axis.X : Axis.Z;

        var pushDirection = Vector3.zero;
        switch (pushAxis)
        {
            case Axis.X:
                pushDirection = new Vector3(localVel.x, 0f, 0f).normalized;
                totalPushLength = GetComponent<Collider>().bounds.extents.x * 2;
                break;
            case Axis.Z:
                pushDirection = new Vector3(0f, 0f, localVel.z).normalized;
                totalPushLength = GetComponent<Collider>().bounds.extents.z * 2;
                break;
        }
        var horizontalVvelocity = transform.TransformDirection(pushDirection * PushSpeed);
        velocity = new Vector3(horizontalVvelocity.x, velocity.y, horizontalVvelocity.z);
        isPushed = true;
        startPosition = transform.position;
    }

    private enum Axis
    {
        NONE,
        X,
        Z
    }
}