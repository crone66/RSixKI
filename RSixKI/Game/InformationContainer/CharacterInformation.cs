namespace RSixKI
{
    public enum CharacterType
    {
        None,
        Player,
        Enemy,
        Hostage
    }

    public struct CharacterInformation
    {
        public string Name;
        public int RowIndex;
        public int ColumnIndex;
        public float Rotation;
        public string[] WeaponNames;
        public int TeamId;
        public int Health;
        public float Speed;
        public float RotationSpeed;
        public CharacterType Type;
    }
}
