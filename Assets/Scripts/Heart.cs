using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour {

	private RawImage image;


	void Start () {
		image = GetComponent<RawImage> ();
	}
	

	void Update () {
		
	}

	public void SetFilled(bool isFilled)
	{
	    if (!image) return;
	    image.color = isFilled ? new Color (image.color.r, image.color.g, image.color.b, 1f) : new Color (image.color.r, image.color.g, image.color.b, 0.1f);
	}
		
}
