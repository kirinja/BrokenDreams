using UnityEditor;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    public float PushSpeed = 5f;

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
            var previousPosition = transform.position;

            transform.position += velocity * Time.deltaTime;

            var pushedLength = 0f;

            switch (pushAxis)
            {
                case Axis.X:
                    if (velocity.x > 0f)
                        pushedLength = transform.position.x - startPosition.x;
                    else
                        pushedLength = startPosition.x - transform.position.x;
                    break;
                case Axis.Z:
                    if (velocity.z > 0f)
                        pushedLength = transform.position.z - startPosition.z;
                    else
                        pushedLength = startPosition.z - transform.position.z;
                    break;
            }

            var didHit = false;
            var hits = Physics.OverlapBox(transform.position, GetComponent<Collider>().bounds.extents);
            foreach (var collider in hits)
                if (collider.CompareTag("Wall"))
                    didHit = true;
                else if (collider.CompareTag("Movable Object"))
                    collider.GetComponent<Pushable>().Push(velocity.normalized);
            if (didHit)
            {
                transform.position = previousPosition;
                isPushed = false;
            }


            if (pushedLength >= totalPushLength)
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
        {
            Debug.Log("Starting box gravity");
            var gravity = Physics.gravity;
            velocity += gravity * Time.deltaTime;
            var previousPosition = transform.position;

            transform.position += new Vector3(0f, velocity.y, 0f) * Time.deltaTime;

            var didHit = false;
            var hits = Physics.OverlapBox(transform.position, GetComponent<Collider>().bounds.extents);
            foreach (var collider in hits)
                if (collider.CompareTag("Wall") ||
                    collider.CompareTag("Movable Object") && collider.gameObject != gameObject)
                {
                    velocity = new Vector3(velocity.x, 0f, velocity.z);
                    didHit = true;
                    break;
                }
            if (didHit)
                transform.position = previousPosition;
        }
        GetComponent<Rigidbody>().position = transform.position;
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