using UnityEditor;
using UnityEngine;

public class AttackableMaterialChanger : MonoBehaviour
{
    private Material material;
    private AbilityColors abilityColor;

	// Use this for initialization
	void Start ()
	{
	    material = GetComponent<Renderer>().sharedMaterial;
        abilityColor = Resources.Load("AbilityColors", typeof(AbilityColors)) as AbilityColors;
        material.color = abilityColor.AttackColor;
	}
	
	// UpdateAbility is called once per frame
	void Update ()
    {
        // this snippet only run when in the editor
#if UNITY_EDITOR
        if (Application.isEditor && !Application.isPlaying)
        {
            material.color = abilityColor.AttackColor;
        }
#endif
    }

    void OnEnable()
    {
        EditorApplication.update += Update;
    }
}
