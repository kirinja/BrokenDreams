using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceCollisionType : MonoBehaviour {

	public enum Mode {Default, Deadzone}
	public Mode terrainType;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public string GetTerrainType(){

		string typeString = "";

		switch (terrainType) {

		case Mode.Default:
			typeString = "Default";
			break;
		case Mode.Deadzone:
			typeString = "Deadzone";
			break;
		default:
			typeString = "";
			break;
		}

		return typeString;

	}
}
