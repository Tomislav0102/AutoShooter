public struct ProjectilePassData
{
    public System.Action<string> onHit;
    public ICharacter attacker;
    public float damage;
    public float moveSpeed;
    public int ricochet;
    public int pierce;
    public int bounce;

    public ProjectilePassData(System.Action<string> onHit, ICharacter attacker, float damage, float moveSpeed, int ricochet = 0, int pierce = 0, int bounce = 0)
    {
        this.onHit = onHit;
        this.attacker = attacker;
        this.damage = damage;
        this.moveSpeed = moveSpeed;
        this.ricochet = ricochet;
        this.pierce = pierce;
        this.bounce = bounce;
    }
}
