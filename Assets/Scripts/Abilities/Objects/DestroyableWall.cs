using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class DestroyableWall : Attackable
{
    public float FadeTime = 0.2f;

    private bool _destroyed;
    private Timer _fadeTimer;
    private Vector3 _originalScale;

    public bool Destroyed { get { return _destroyed; } }

    private void Start()
    {
        _destroyed = false;
        _fadeTimer = new Timer(FadeTime);
        var childTranforms = GetComponentsInChildren<Transform>();
        if (childTranforms.Length > 1)
            _originalScale = childTranforms[1].localScale;
    }


    public override void Damage(int damage = 1)
    {
        if (_destroyed) return;

        _destroyed = true;
        GetComponent<Collider>().enabled = false;
        GetComponent<AudioSource>().Play();

        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (var body in rigidBodies)
        {
            body.isKinematic = false;
        }
    }


    private void Update()
    {
        if (!_destroyed) return;

        if (_fadeTimer.Update(Time.deltaTime))
        {
            Destroy(gameObject);
        }
        else
        {
            var childTransforms = GetComponentsInChildren<Transform>();
            foreach (var childTransform in childTransforms)
            {
                if (childTransform.gameObject != gameObject)
                childTransform.transform.localScale = Vector3.Lerp(_originalScale, Vector3.zero, _fadeTimer.PercentDone);
            }
        }
    }
}