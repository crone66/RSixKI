namespace RSixKI
{
    public struct WeaponInformation
    {
        public string Name;
        public Weapon.WeaponSlot Slot;
        public float Firerate;
        public float ReloadTime;
        public int ClipSize;
        public int ClipCount;
        public Weapon.AmmoTyp AmmoType;
        public Weapon.EquipmentType EquipmentType;
        public string ProjectileName;
        public int Mobility;

        public WeaponInformation(string name, int clipSize, int clipCount, float firerate, float reloadTime, int mobility, Weapon.AmmoTyp ammoType, Weapon.WeaponSlot slot, Weapon.EquipmentType equipType, string projectileName)
        {
            Name = name;
            Slot = slot;
            Firerate = firerate;
            ClipCount = clipCount;
            ClipSize = clipSize;
            ReloadTime = reloadTime;
            AmmoType = ammoType;
            EquipmentType = equipType;
            ProjectileName = projectileName;
            Mobility = mobility;
        }
    }
}
