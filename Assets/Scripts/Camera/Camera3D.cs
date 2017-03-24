using UnityEngine;

/// <summary>
/// This is a fairly simple camera script that makes the camera orbit around a Target
/// </summary>
public class Camera3D : MonoBehaviour
{
    [Tooltip("The Target the camera will orbit around")]
    public Transform Target;
    
    [Tooltip("Camera speed horizontally")]
    public float HorizontalSensitivity = 40.0f;
    [Tooltip("Camera speed vertically")]
    public float VerticalSensitivity = 40.0f;
    [Tooltip("Camera speed horizontally (for gamepad)")]
    public float HorizontalSensitivityGamepad = 40.0f;
    [Tooltip("Camera speed vertically (for gamepad)")]
    public float VerticalSensitivityGamepad = 40.0f;

    [Tooltip("The minimum angle the camera can move vertically, measured in degrees")]
    public float ClampCameraMin = -15f;
    [Tooltip("The maximum angle the camera can move vertically, measured in degrees")]
    public float ClampCameraMax = 40f;
    
    [Tooltip("The preferred distance of the camera, relative to the Target, in Unity units")]
    public float PreferredDistance = 10.0f;
    
    [Tooltip("The speed which the Camera zooms in if an object is obstructing it")]
    public float CameraAccelerationIn = 4.0f;
    [Tooltip("The speed which the Camera zooms out if there is a clear path to do so")]
    public float CameraAccelerationOut = 2.0f;
    
    [Tooltip("What should the Camera collision detecting check against")]
    public LayerMask LayerMask;
    
    public bool InvertX;
    public bool InvertY;

    private bool _isUsingController;
    private float _x;
    private float _y;
    private float _actualDistance;

    [HideInInspector]
    public float Margin = 0.3f;
    public float OffsetY = 0.1f;
    
    private Transform _transform;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _transform = transform;

        // Getting the angles to start from
        Vector3 angles = Target.eulerAngles;
        _x = angles.y;
        _y = angles.x;
        _actualDistance = PreferredDistance;

        // this should probably be moved to a seperate helper class that allows us to determine what kind of controller we are using, if any
        // right now we're only checking if there is ANY controller plugged in and assuming it is an Xbox 360 controller
        // it would be better to split this into possibly three categories: PS4, Xbox 360, No Controller
        _isUsingController = Input.GetJoystickNames().Length != 0;
        
    }

    void LateUpdate()
    {
        if (!Target) return;

        if (_isUsingController)
        {
            _x += Input.GetAxis("Right Horizontal") * HorizontalSensitivityGamepad * Time.deltaTime * (InvertX ? 1 : -1);
            _y -= Input.GetAxis("Right Vertical") * VerticalSensitivityGamepad * Time.deltaTime * (InvertY ? 1 : -1);
        }
        else
        {
            _x += Input.GetAxisRaw("Mouse X") * HorizontalSensitivity * Time.deltaTime * (InvertX ? 1 : -1);
            _y -= Input.GetAxisRaw("Mouse Y") * VerticalSensitivity * Time.deltaTime * (InvertY ? 1 : -1);
        }
        
        _y = ClampAngle(_y, ClampCameraMin, ClampCameraMax);

        // this might seem confusing but that's because yaw and pitch, X plane corresponds to moving UP/DOWN and Y lane corresponds to LEFT/RIGHT
        Quaternion rotation = Quaternion.Euler(_y, _x, 0);

        var rayLength = ((Target.position + new Vector3(0, OffsetY, 0)) - _transform.position).magnitude;

        RaycastHit rayHit;
        var hit = Physics.Raycast(Target.position + new Vector3(0, OffsetY, 0), -_transform.forward, out rayHit, rayLength, LayerMask);
        
        if (hit)
        {
            Debug.DrawLine(Target.position + new Vector3(0, OffsetY, 0), _transform.position, Color.magenta);
            
            float steps = rayHit.distance * CameraAccelerationIn * Time.deltaTime;
            _actualDistance -= steps;
        }
        else
        {
            Debug.DrawLine(Target.position, _transform.position, Color.green);

            // when zooming out we're gonna check a little further ahead than the total distance, to see if we can actually zoom out or not
            var cantZoomOut = Physics.Raycast(Target.position + new Vector3(0, OffsetY, 0), -_transform.forward, out rayHit, rayLength + Margin, LayerMask);
            
            if (_actualDistance < PreferredDistance && !cantZoomOut)
            {
                //_actualDistance += _actualDistance * CameraAccelerationOut * Time.deltaTime;
                _actualDistance = Mathf.Lerp(_actualDistance, PreferredDistance, Time.deltaTime * CameraAccelerationOut);
            }
        }

        _transform.rotation = rotation;
        _transform.position = rotation * new Vector3(0.0f, OffsetY, -_actualDistance) + Target.position;
    }


    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}