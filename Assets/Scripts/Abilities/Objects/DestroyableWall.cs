public class DestroyableWall : Attackable
{
    public override void Damage(int damage = 1)
    {
        Destroy(this.gameObject);
    }
}