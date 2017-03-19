namespace RSixKI
{
    public struct WeaponContainer
    {
        public WeaponInformation[] Weapons;
        public ProjectileInformation[] Projectiles;

        public WeaponContainer(WeaponInformation[] weapons, ProjectileInformation[] projectiles)
        {
            Weapons = weapons;
            Projectiles = projectiles;
        }
    }
}
