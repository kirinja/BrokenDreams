public class DestroyableWall : Attackable
{
    public override void Damage()
    {
        Destroy(this.gameObject);
    }
}