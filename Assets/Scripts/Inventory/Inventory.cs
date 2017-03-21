using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	public int MaxSlots = 4;

	public InventorySlotUI[] slots; //HÄR SKA ABILITY KLASSEN VARA

	private void Awake() {
		/*slots = new InventorySlotUI[4];
		slots [0] = new InventorySlotUI ();
		slots [1] = new InventorySlotUI ();
		slots [2] = new InventorySlotUI ();
		slots [3] = new InventorySlotUI ();
*/
	}

	void Start () {
		slots[0].Selected();
        GameObject.Find("Player").GetComponent<Controller3D>().SetAbility(0);
    }

	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			slots[0].Selected ();
			slots [1].Unselected ();
			slots [2].Unselected ();
			slots [3].Unselected ();

		    GameObject.Find("Player").GetComponent<Controller3D>().SetAbility(0);

		} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			slots[0].Unselected ();
			slots [1].Selected ();
			slots [2].Unselected ();
			slots [3].Unselected ();

            GameObject.Find("Player").GetComponent<Controller3D>().SetAbility(1);
        }
		else if (Input.GetKeyDown (KeyCode.Alpha3)) {
			slots[0].Unselected ();
			slots [1].Unselected ();
			slots [2].Selected ();
			slots [3].Unselected ();

            GameObject.Find("Player").GetComponent<Controller3D>().SetAbility(2);
        }
		else if (Input.GetKeyDown (KeyCode.Alpha4)) {
			slots[0].Unselected ();
			slots [1].Unselected ();
			slots [2].Unselected ();
			slots [3].Selected ();

            GameObject.Find("Player").GetComponent<Controller3D>().SetAbility(3);
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


