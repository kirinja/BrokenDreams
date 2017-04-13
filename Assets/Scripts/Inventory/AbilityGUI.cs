using UnityEngine;

public class AbilityGUI : MonoBehaviour {

	public int MaxSlots = 4;
    public AbilityGUISlot[] slots;
    public PlayerAttributes PlayerAttributes;

    private int activeSlots;

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