using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    private enum Axis { NONE, X, Z }

    public float PushSpeed = 5f;

    private Material material;
    private AbilityColors abilityColor;
    private Vector3 velocity;
    private bool isPushed;
    private float totalPushLength;
    private Axis pushAxis;
    private Vector3 startPosition;

    // Use this for initialization
    void Start()
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
    void Update()
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
                    {
                        pushedLength = transform.position.x - startPosition.x;
                    }
                    else
                    {
                        pushedLength = startPosition.x - transform.position.x;
                    }
                    break;
                case Axis.Z:
                    if (velocity.z > 0f)
                    {
                        pushedLength = transform.position.z - startPosition.z;
                    }
                    else
                    {
                        pushedLength = startPosition.z - transform.position.z;
                    }
                    break;
            }

            var didHit = false;
            var hits = Physics.OverlapBox(transform.position, GetComponent<Collider>().bounds.extents);
            foreach (var collider in hits)
            {
                if (collider.CompareTag("Wall"))
                {
                    didHit = true;
                }
                else if (collider.CompareTag("Movable Object"))
                {
                    collider.GetComponent<Pushable>().Push(velocity.normalized);
                }
            }
            if (didHit)
            {
                transform.position = previousPosition;
                isPushed = false;
            }

            

            if (pushedLength >= totalPushLength)
            {
                isPushed = false;
            }
        }
        {
            Debug.Log("Starting box gravity");
            var gravity = Physics.gravity;
            var previousPosition = transform.position;

            transform.position += gravity * Time.deltaTime;

            var didHit = false;
            var hits = Physics.OverlapBox(transform.position, GetComponent<Collider>().bounds.extents);
            foreach (var collider in hits)
            {
                if (collider.CompareTag("Wall") || collider.CompareTag("Movable Object") && collider.gameObject != this.gameObject)
                {
                    didHit = true;
                    break;
                }
            }
            if (didHit)
            {
                transform.position = previousPosition;
            }
        }
        GetComponent<Rigidbody>().position = transform.position;
    }

    void UpdateColor()
    {
        // this snippet only run when in the editor
#if UNITY_EDITOR
        if (Application.isEditor && !Application.isPlaying)
        {
            material.color = abilityColor.PushColor;
        }
#endif
    }

    void OnEnable()
    {
        EditorApplication.update += UpdateColor;
    }

    public void Push(Vector3 direction)
    {
        if (isPushed) return;

        var localVel = transform.InverseTransformDirection(direction);
        pushAxis = (Mathf.Abs(localVel.x) > Mathf.Abs(localVel.z)) ? Axis.X : Axis.Z;

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
        velocity = transform.TransformDirection(pushDirection * PushSpeed);
        isPushed = true;
        startPosition = transform.position;
    }
}
