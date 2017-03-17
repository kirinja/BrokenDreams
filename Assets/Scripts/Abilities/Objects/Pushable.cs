using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    private Material material;

    private AbilityColors abilityColor;

	// Use this for initialization
	void Start ()
	{
	    material = GetComponent<Renderer>().sharedMaterial;
        abilityColor = Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors;
        material.color = abilityColor.PushColor;
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

    }

    void OnEnable()
    {
        EditorApplication.update += Update;
    }
}
