namespace RSixKI
{
    public struct ProjectileInformation
    {
        public string Name;
        public float Damage;
        public float SuspressedDamage;
        public float DamageReductionPerUnit;
        public float Speed;
        public float LifeTime;
        public bool RayCastShot;

        public ProjectileInformation(string name, float damage, float suspressedDamage, float damageReductionPerUnit, float speed, float lifeTime, bool rayCastShot)
        {
            Name = name;
            Damage = damage;
            SuspressedDamage = suspressedDamage;
            DamageReductionPerUnit = damageReductionPerUnit;
            Speed = speed;
            LifeTime = lifeTime;
            RayCastShot = rayCastShot;
        }
    }
}
