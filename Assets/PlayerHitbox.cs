using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    private Timer _hitboxTimer;
    public float Width = 1f;
    public float Height = 0.8f;
    public float LastingTime = 0.3f;

    private readonly float depth = 1f;


    // Use this for initialization
    private void Start()
    {
        _hitboxTimer = new Timer(LastingTime);
    }


    // Update is called once per fram
    private void Update()
    {
        if (_hitboxTimer.Update(Time.deltaTime))
        {
            Destroy(gameObject);
        }
        else
        {
            var hits = Physics.OverlapBox(transform.position, new Vector3(Width, Height, depth));

            foreach (var gameObject in hits)
            {
                var hitObject = gameObject.GetComponent<Attackable>();
                if (hitObject)
                    hitObject.Damage();
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(Width, Height, depth));
    }
}