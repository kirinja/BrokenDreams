using UnityEngine;


internal class DashState3D : ICharacterState3D
{
    private readonly Controller3D _controller;
    private readonly Timer _dashTimer;
    private readonly Vector2 _previousVelocity;
    private bool _dashButtonUp;


    public DashState3D(Controller3D controller)
    {
        _controller = controller;
        _previousVelocity = controller.Velocity;

        _dashTimer = new Timer(controller.GetComponent<PlayerAttributes>().DashTime);
        UpdateRotation();

        var attributes = controller.GetComponent<PlayerAttributes>();
        var speed = attributes.DashLength / attributes.DashTime;

        controller.Velocity = Mathf.Sign(controller.Forward.x) * speed * Vector2.right;

        _dashButtonUp = false;
    }


    public bool AttemptStateSwitch(CharacterStateSwitch3D state)
    {
        return false;
    }


    public void Enter()
    {
        _dashTimer.Reset();

        _controller.transform.Find("Dash").Find("DashTrail").GetComponent<ParticleSystem>().Play();
        _controller.transform.Find("Dash").Find("Sparkle").GetComponent<ParticleSystem>().Play();
    }


    public void Update()
    {
        _dashButtonUp = Input.GetButtonUp("Use Ability 4");
        if (_dashTimer.Update(Time.deltaTime) || _dashButtonUp)
            _controller.ChangeCharacterState(new CharacterStateSwitch3D(new AirState3D(_controller)));
    }


    public void Exit()
    {
        _controller.Velocity = Mathf.Abs(_previousVelocity.x) <= _controller.Attributes.MaxSpeed
            ? new Vector2(_previousVelocity.x, 0f)
            : new Vector2(Mathf.Sign(_previousVelocity.x) * _controller.GetComponent<PlayerAttributes>().MaxSpeed, 0f);

        _controller.transform.Find("Dash").Find("DashTrail").GetComponent<ParticleSystem>().Stop();
        _controller.transform.Find("Dash").Find("Sparkle").GetComponent<ParticleSystem>().Stop();
    }


    public void LateUpdate()
    {
    }


    public void FixedUpdate()
    {
    }


    public CharacterStateSwitch3D HandleCollisions(CollisionFlags collisionFlags)
    {
        if (collisionFlags == CollisionFlags.Sides)
        {
            _controller.Velocity = new Vector2(_controller.GetComponent<PlayerAttributes>().MaxSpeed, 0f);

            return new CharacterStateSwitch3D(new AirState3D(_controller));
        }

        return new CharacterStateSwitch3D();
    }


    private void UpdateRotation()
    {
        var desiredAngle = Mathf.Sign(_controller.Forward.x) > 0f ? 90f : -90f;
        _controller.transform.eulerAngles = new Vector3(0f, desiredAngle, 0f);
    }
}