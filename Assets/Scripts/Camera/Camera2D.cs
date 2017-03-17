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

    //private Controller2D _controller2D;

	// Use this for initialization
	void Start ()
	{
	    //_targetCollider = Target.GetComponent<BoxCollider2D>();
        //if (!_targetCollider)
        //    throw new NullReferenceException("The target need a 2D BoxCollider");

	    _targetCollider = Target.GetComponent<Collider>();


	    //_controller2D = Target.GetComponent<Controller2D>();
        _cameraFocusArea = new FocusArea(0, 0, Size.x, Size.y);
        //_targetFocusArea = new FocusArea(0, 0, _targetCollider.size.x, _targetCollider.size.y);

        _targetFocusArea = new FocusArea(0, 0, _targetCollider.bounds.size.x, _targetCollider.bounds.size.y);
        //CalculateDirection();
    }

    void LateUpdate()
    {
        if (!Target) return;

        CalculateDirection();
        CalculateTargetArea();
        CalculateCameraArea();
        
        if (!_cameraFocusArea.Contains(_targetFocusArea))
        {
            if (_targetFocusArea.Left <= _cameraFocusArea.Left) // LEFT
                transform.position = new Vector3(_targetFocusArea.Left - Offset.x + _cameraFocusArea.Width/2, transform.position.y, -Offset.z);
            else if (_targetFocusArea.Right >= _cameraFocusArea.Right) // RIGHT
                transform.position = new Vector3(_targetFocusArea.Right - Offset.x - _cameraFocusArea.Width/2, transform.position.y, -Offset.z);

            
            if (_targetFocusArea.Top >= _cameraFocusArea.Top) // UP
                transform.position = new Vector3(transform.position.x, _targetFocusArea.Top - Offset.y - _cameraFocusArea.Height/2, -Offset.z);
            else if (_targetFocusArea.Bot <= _cameraFocusArea.Bot) // BOT
               transform.position = new Vector3(transform.position.x, _targetFocusArea.Bot - Offset.y + _cameraFocusArea.Height/2, -Offset.z);
        }
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

/**
         if (!_cameraFocusArea.Contains(_targetRectangle))
        {
            //transform.position = Vector2.Lerp(transform.position,
            //    new Vector2(Target.position.x, Target.position.y) - Offset, Time.deltaTime);
            //transform.position = Vector3.Lerp(transform.position, Target.position - new Vector3(Offset.x, Offset.y, transform.position.z), Time.deltaTime);

            // TODO This somewhat works as a fallback (it is jerky when jumping and that's because the magnitude gets bigger due to 2 axis at the same time)
            //transform.position = Vector2.Lerp(transform.position, new Vector2(Target.position.x, Target.position.y) - Offset, Time.deltaTime * Mathf.Abs(_controller2D.Velocity.magnitude));

            // if the focus _cameraFocusArea and the targetbounds dont intersect(overlap) then we need to move the camera so it focuses the object
            // we want to use delta time and a timed variable here, say it should take 2 seconds to center the object again
            // we also need to calculate the difference between the target and the focus _cameraFocusArea, so we know how long we have to move
            // after we have a distance we need to calculate the steps and then move with those steps
            
            //var distX = _cameraFocusArea.Center.x - (_targetRectangle.Center.x + (Offset.x * -Mathf.Sign(Target.right.x)));
            //var distY = _cameraFocusArea.Center.y + (_targetRectangle.Center.y + Offset.y);
            // first lets get the distance in x and y
            var distX = (_cameraFocusArea.Position.x - _targetRectangle.Position.x);
            var distY = _cameraFocusArea.Position.y - _targetRectangle.Position.y;


            // BUG it doesnt work when the coords go towards negativ values
            // BUG it just keeps adding (it might be the collision detection that is weird?)

            //Debug.Log(distX + " - " + distY);

            // next we need to caluculate the step value, based on delta time and the given time scale we want
            
            // this is the amount of frames we have (we have to make sure to either decrease FollowTime and reset when we're done)
            // might need to take velocity into account?
            var amountOfFrames = (1 / Time.deltaTime) * _timer;

            // we have the amount of frames we should move over, and we have the distance
            var stepsX = (distX / amountOfFrames);
            var stepsY = distY / amountOfFrames;
            //Debug.Log(stepsX + " - " + stepsY + " : " + distX + " - " + distY + " : " + amountOfFrames + " : " + Time.deltaTime);
            
            // could extend vector2 from here
            transform.position = new Vector2(transform.position.x - stepsX, transform.position.y - stepsY);
            //Debug.Log(transform.position + " : " + stepsX + " - " + stepsY);
            //Debug.Log(distX);
            //transform.position = new Vector2();
            _timer -= Time.deltaTime;


            // calculate distance we need to travel
            //var distx = _cameraFocusArea.TopLeft.x - _targetRectangle.TopLeft.x;
            //var disty = _cameraFocusArea.TopLeft.y - _targetRectangle.TopLeft.y;
        }
        else
        {
            _timer = FollowTime;
        }
*/
