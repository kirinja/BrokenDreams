using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventorySlotUI : MonoBehaviour {

	public float RightPadding;

	private RectTransform rectTransform;

	// public InventorySlot Slot; // HÄR SKA ABILITY KLASSEN VARA

	private CanvasRenderer canvasRenderer;

	public Image icon;

	void Start () {
		
	}
	

	private void Update () {


	}

	public void Selected() {
		transform.localScale = new Vector3 (1.5f, 1.5f, 1.5f );
	}

	public void Unselected (){
		transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
	}
}
