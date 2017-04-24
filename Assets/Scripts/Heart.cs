using UnityEngine;
using UnityEngine.UI;


public class Heart : MonoBehaviour
{
    private RawImage _image;


    private void Start()
    {
        _image = GetComponent<RawImage>();
    }


    private void Update()
    {
    }


    public void SetFilled(bool isFilled)
    {
        _image.color = isFilled ? new Color(_image.color.r, _image.color.g, _image.color.b, 1f) : new Color(_image.color.r, _image.color.g, _image.color.b, 0.1f);
    }
}
	public void SetFilled(bool isFilled)
	{
	    if (!image) return;
	    image.color = isFilled ? new Color (image.color.r, image.color.g, image.color.b, 1f) : new Color (image.color.r, image.color.g, image.color.b, 0.1f);
	}
		
}
