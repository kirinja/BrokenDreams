using UnityEditor.Animations;
using UnityEngine;

public class AbilityGUI : MonoBehaviour {

	public int MaxSlots = 4;
    public AbilityGUISlot[] slots;
    public PlayerAttributes PlayerAttributes;

    private int activeSlots;
    public RuntimeAnimatorController[] animations;

	void Start () {
		for (var i = 0; i < PlayerAttributes.Abilities.Count; i++)
        {
			slots [i].gameObject.SetActive (true);
			activeSlots = i + 1;
		}
    }

	public void addAbility(){
	    if (activeSlots >= MaxSlots) return;
        
	    slots [activeSlots++].gameObject.SetActive (true);
        // play animation here, have an array of them or something?
        // think we can get away with an array of animation controllers and then just enable them one after one
	    GetComponentInParent<Animator>().runtimeAnimatorController = animations[activeSlots - 1];
	    GetComponentInParent<Animator>().enabled = true;
	}

    public void ShowAbilitiesUsed(bool[] abilities)
    {
        for (var i = 0; i < activeSlots; ++i)
        {
            if (abilities[i])
            {
                slots[i].Use();
            }
        }
    }
}