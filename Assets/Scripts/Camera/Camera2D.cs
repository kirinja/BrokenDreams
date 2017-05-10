using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera2D : MonoBehaviour
{
    public Transform Target;
    public Vector2 Size;
    public Vector3 Offset;
    public float SmoothFactor = 7.0f;

    private FocusArea _targetFocusArea;

    private FocusArea _cameraFocusArea;
    private Collider _targetCollider;

    private float _newOffsetX;
    private bool _facingLeft;
    private bool _prevFacingLeft;

    private float _origOffsetX;

    private float smoothTemp;
    
	// Use this for initialization
	void Start ()
	{
	    _targetCollider = Target.GetComponent<Collider>();
        
        _cameraFocusArea = new FocusArea(0, 0, Size.x, Size.y);

        _targetFocusArea = new FocusArea(0, 0, _targetCollider.bounds.size.x, _targetCollider.bounds.size.y);
        CalculateDirection();
	    _newOffsetX = Offset.x;
	    _origOffsetX = Offset.x;

	    Offset.z = transform.position.z;
	}

    void LateUpdate()
    {
        if (!Target) return;
        
        
        CalculateTargetArea();
        CalculateCameraArea();
        Offset.z = transform.position.z;
        transform.position = new Vector3(transform.position.x, transform.position.y, Offset.z);

        //if (_cameraFocusArea.Contains(_targetFocusArea)) return;

        if (!_cameraFocusArea.ContainsX(_targetFocusArea))
        {
            // try to place player in the center of the camera focus area instead of just moving it along the player

            FollowTargetX();

            CalculateDirection();
            CalculateOffset();
        }
        if (!_cameraFocusArea.ContainsY(_targetFocusArea))
            FollowTargetY();
        
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
        _facingLeft = !(Target.forward.x > 0);

        if (_facingLeft && !_prevFacingLeft)
            FlipDirection();
        else if (!_facingLeft && _prevFacingLeft)
            FlipDirection();

        _prevFacingLeft = _facingLeft;
    }

    void FlipDirection()
    {
        _origOffsetX *= -1;
        _newOffsetX = _origOffsetX;
    }

    void CalculateOffset()
    {
        //Offset.x = Mathf.SmoothStep(Offset.x, _newOffsetX, Time.deltaTime* SmoothFactor);
        Offset.x = Mathf.SmoothDamp(Offset.x, _newOffsetX, ref smoothTemp, SmoothFactor);
        
        if (Mathf.Abs(Offset.x - _newOffsetX) < 0.1f)
            Offset.x = _newOffsetX;
    }

    void FollowTargetX()
    {
        // there is a slight jitter here, might be the offset values
        if (_targetFocusArea.Left <= _cameraFocusArea.Left) // LEFT
        {
            transform.position = new Vector3(_targetFocusArea.Left - Offset.x + _cameraFocusArea.Width / 2,
                transform.position.y, Offset.z);
        }
        else if (_targetFocusArea.Right >= _cameraFocusArea.Right) // RIGHT
        {
            transform.position = new Vector3(_targetFocusArea.Right - Offset.x - _cameraFocusArea.Width / 2,
                transform.position.y, Offset.z);
        }
    }

    void FollowTargetY()
    {
        if (_targetFocusArea.Top >= _cameraFocusArea.Top) // UP
            transform.position = new Vector3(transform.position.x, _targetFocusArea.Top - Offset.y - _cameraFocusArea.Height / 2, Offset.z);
        else if (_targetFocusArea.Bot <= _cameraFocusArea.Bot) // BOT
            transform.position = new Vector3(transform.position.x, _targetFocusArea.Bot - Offset.y + _cameraFocusArea.Height / 2, Offset.z);
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

    public bool ContainsX(FocusArea other)
    {
        return Left <= other.Left && Right >= other.Right;
    }

    public bool ContainsY(FocusArea other)
    {
        return Top >= other.Top && Bot <= other.Bot;
    }

    public override string ToString()
    {
        return "X: " + X + " Y: " + Y + " W: " + Width + " H: " + Height;
    }
}