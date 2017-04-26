using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class DestroyableWall : Attackable
{
    public override void Damage(int damage = 1)
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        var source = GetComponent<AudioSource>();
        source.Play();
        Destroy(this.gameObject, source.clip.length);
    }
}