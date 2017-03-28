using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	public int MaxSlots = 4;

	private int activeSlots;
    private int currentSlot;

	public InventorySlotUI[] slots; //HÄR SKA ABILITY KLASSEN VARA
    public Controller3D controller3D;

	private void Awake() {
		/*slots = new InventorySlotUI[4];
		slots [0] = new InventorySlotUI ();
		slots [1] = new InventorySlotUI ();
		slots [2] = new InventorySlotUI ();
		slots [3] = new InventorySlotUI ();
*/
	}

	void Start () {
		var playerAttribute = controller3D.GetComponent<PlayerAttributes> ();

		for (var i = 0; i < playerAttribute.Abilities.Count; i++) {
			slots [i].gameObject.SetActive (true);
			activeSlots = i + 1;
		}

	    currentSlot = 0;
			
		if (activeSlots > 0) {
			slots[0].Selected();
			controller3D.SetAbility(0);
		}
    }

	void Update () {
		if (Input.GetButtonDown("Select Ability 1") && activeSlots > 0) {
			slots[0].Selected ();
			slots[1].Unselected ();
			slots[2].Unselected ();
			slots[3].Unselected ();

		    currentSlot = 0;
            controller3D.SetAbility(currentSlot);

		} else if (Input.GetButtonDown("Select Ability 2") && activeSlots > 1) {
			slots[0].Unselected ();
			slots[1].Selected ();
			slots[2].Unselected ();
			slots[3].Unselected ();

            currentSlot = 1;
            controller3D.SetAbility(currentSlot);
        }
		else if (Input.GetButtonDown ("Select Ability 3") && activeSlots > 2) {
			slots[0].Unselected ();
			slots[1].Unselected ();
			slots[2].Selected ();
			slots[3].Unselected ();

            currentSlot = 2;
            controller3D.SetAbility(currentSlot);
        }
		else if (Input.GetButtonDown ("Select Ability 4") && activeSlots > 3) {
			slots[0].Unselected ();
			slots[1].Unselected ();
			slots[2].Unselected ();
			slots[3].Selected ();

            currentSlot = 3;
            controller3D.SetAbility(currentSlot);
        }
        else if (Input.GetButtonDown("Next Ability") && activeSlots > currentSlot + 1)
		{
		    ++currentSlot;
		    foreach (var slot in slots)
		    {
		        slot.Unselected();
		    }
            slots[currentSlot].Selected();
            controller3D.SetAbility(currentSlot);
        }
        else if (Input.GetButtonDown("Previous Ability") && currentSlot > 0)
        {
            --currentSlot;
            foreach (var slot in slots)
            {
                slot.Unselected();
            }
            slots[currentSlot].Selected();
            controller3D.SetAbility(currentSlot);
        }
    }

	public void addAbility(){
		if (activeSlots < MaxSlots) {
			if(activeSlots == 0){
				slots [activeSlots].Selected ();
				controller3D.SetAbility(0);
			}
			slots [activeSlots++].gameObject.SetActive (true);
		}

	}

//	public InventorySlot GetSlot (int slotIndex) {
	//	if(slotIndex < 0 || slotIndex >= slotCount) {
	//		var message = string.Format ("Slot index must be between [0-{0}).");
	//		throw new ArgumentOutOfRangeException ("slotIndex", slotIndex, message);
		}

	//	return MaxSlots [slotIndex];



	//public void AddItem(Item item) {
	//	if (item == null) {
	//		throw new ArgumentNullException ("item");
	//	}

	//	var slotIndex = GetSlotIndexForItem (item);
	//	if (slotIndex != -1) {
	//		slots [slotIndex].AddItem (item);
	//	}
	//}


