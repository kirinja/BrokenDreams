using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class DestroyableWall : Attackable
{
    public float FadeTime = 0.2f;

    private bool _destroyed;
    private Timer _fadeTimer;

    public bool Destroyed { get { return _destroyed; } }

    private void Start()
    {
        _destroyed = false;
        _fadeTimer = new Timer(FadeTime);
    }


    public override void Damage(int damage = 1)
    {
        if (_destroyed) return;

        _destroyed = true;
        
        GetComponent<Collider>().enabled = false;

        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (var body in rigidBodies)
        {
            body.isKinematic = false;
        }

        var source = GetComponent<AudioSource>();
        source.Play();
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
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var childRenderer in renderers)
            {
                childRenderer.material.color = new Color(childRenderer.material.color.r, childRenderer.material.color.g,
                    childRenderer.material.color.b, 1 - _fadeTimer.PercentDone);
            }
        }
    }
}