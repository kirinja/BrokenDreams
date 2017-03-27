using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    public float PushSpeed = 5f;

    private Material material;
    private AbilityColors abilityColor;
    private Vector3 velocity;
    private bool isPushed;
    private float totalPushLength, pushedLength;

	// Use this for initialization
	void Start ()
	{
	    material = GetComponent<Renderer>().sharedMaterial;
        abilityColor = Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors;
        material.color = abilityColor.PushColor;
        velocity = Vector3.zero;
	    totalPushLength = 0f;
	    isPushed = false;
	}
	
	// UpdateTime is called once per frame
	void Update ()
    {
        // this snippet only run when in the editor
#if UNITY_EDITOR
        if (Application.isEditor && !Application.isPlaying)
        {
            material.color = abilityColor.PushColor;
        }
#endif


        if (isPushed)
        {
            //var collider = GetComponent<Collider>();

            transform.position += velocity * Time.deltaTime;
            GetComponent<Rigidbody>().position = transform.position;
            GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, 0f);
            pushedLength += velocity.magnitude * Time.deltaTime;
            

            if (pushedLength >= totalPushLength)
            {
                isPushed = false;
            }
        }
    }

    void OnEnable()
    {
        EditorApplication.update += Update;
    }

    public void Push(Vector3 direction)
    {
        if (isPushed) return;

        var localVel = transform.InverseTransformDirection(direction);
        var onX = (Mathf.Abs(localVel.x) > Mathf.Abs(localVel.z));
        var pushDirection = (onX ? new Vector3(localVel.x, 0f, 0f)
            : new Vector3(0f, 0f, localVel.z)).normalized;
        velocity = transform.TransformDirection(pushDirection * PushSpeed);
        isPushed = true;
        totalPushLength = onX ? transform.lossyScale.x : transform.lossyScale.z;
        pushedLength = 0f;
        Debug.Log(velocity);
    }
}
