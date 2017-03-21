using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera2D : MonoBehaviour
{
    public Transform Target; // this we can find from the scene later
    public Vector2 Size;
    public Vector3 Offset;
    private FocusArea _targetFocusArea;

    private FocusArea _cameraFocusArea;
    //private BoxCollider2D _targetCollider;
    private Collider _targetCollider;
    // might replace the above with a size that you manually set, at least for the test
    

	// Use this for initialization
	void Start ()
	{

	    _targetCollider = Target.GetComponent<Collider>();
        
        _cameraFocusArea = new FocusArea(0, 0, Size.x, Size.y);

        _targetFocusArea = new FocusArea(0, 0, _targetCollider.bounds.size.x, _targetCollider.bounds.size.y);
        //CalculateDirection();
    }

    void LateUpdate()
    {
        if (!Target) return;

        CalculateDirection();
        CalculateTargetArea();
        CalculateCameraArea();

        if (_cameraFocusArea.Contains(_targetFocusArea)) return;

        if (_targetFocusArea.Left <= _cameraFocusArea.Left) // LEFT
            transform.position = new Vector3(_targetFocusArea.Left - Offset.x + _cameraFocusArea.Width/2, transform.position.y, -Offset.z);
        else if (_targetFocusArea.Right >= _cameraFocusArea.Right) // RIGHT
            transform.position = new Vector3(_targetFocusArea.Right - Offset.x - _cameraFocusArea.Width/2, transform.position.y, -Offset.z);

            
        if (_targetFocusArea.Top >= _cameraFocusArea.Top) // UP
            transform.position = new Vector3(transform.position.x, _targetFocusArea.Top - Offset.y - _cameraFocusArea.Height/2, -Offset.z);
        else if (_targetFocusArea.Bot <= _cameraFocusArea.Bot) // BOT
            transform.position = new Vector3(transform.position.x, _targetFocusArea.Bot - Offset.y + _cameraFocusArea.Height/2, -Offset.z);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(_targetFocusArea.Center, new Vector2(_targetFocusArea.Width, _targetFocusArea.Height));

        Gizmos.color = new Color(0, 0, 1, .5f);
        Gizmos.DrawCube(_cameraFocusArea.Center, new Vector2(_cameraFocusArea.Width, _cameraFocusArea.Height));
    }

    void CalculateTargetArea()
    {
        _targetFocusArea.SetPosition(Target.transform.position.x - Target.transform.localScale.x /2, Target.transform.position.y + Target.transform.localScale.y/2);
    }

    void CalculateCameraArea()
    {
        _cameraFocusArea.SetPosition(transform.position.x - _cameraFocusArea.Width / 2 + Offset.x, transform.position.y + _cameraFocusArea.Height / 2 + Offset.y);
    }

    void CalculateDirection()
    {
        //Offset.x = Offset.x * -Mathf.Sign(Target.right.x); // we cant use .right.x (mmight need to introduce a direction variable in controller2d or base it on velocity (method from controller2d))

        //Offset.x = _controller2D.GetHorizontalDirection(_controller2D.Velocity).ToInt();

        // on a direction change (LEFT TO RIGHT and vice versa) we need to smoothly lerp the offset (so it doesnt jump around)
        // we only need to do this on the X axis
        // for now we ignore this and fix it later
        // We could even use a "look ahead" value that only aligns on the X axis and do calulcations on this value and use it in camera placement
    }

}

struct FocusArea
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }

    public float Left { get { return X; } }
    public float Right { get { return X + Width; } }
    public float Top { get { return Y; } }
    public float Bot { get { return Y - Height; } }

    public Vector2 Center { get { return new Vector2(X + Width / 2, Y - Height / 2);} }
    public Vector2 Position { get {return new Vector2(X, Y);} }

    public FocusArea(float x, float y, float width, float height) : this()
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public void SetPosition(float x, float y)
    {
        X = x;
        Y = y;
    }

    public bool Contains(FocusArea other)
    {
        return (Left <= other.Left && Right >= other.Right) && (Top >= other.Top && Bot <= other.Bot);
    }

    public override string ToString()
    {
        return "X: " + X + " Y: " + Y + " W: " + Width + " H: " + Height;
    }
}