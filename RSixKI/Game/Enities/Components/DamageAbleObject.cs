using System;
using Microsoft.Xna.Framework;

namespace RSixKI
{
    public class DamageAbleObject : EntityComponent
    {
        private float health;
        private float maxHealth;
        private bool destroyOnDeath;

        public event EventHandler<HealthChangingArgs> OnHealthChanging;
        public event EventHandler<HealthChangedArgs> OnHealthChanged;
        public event EventHandler<HealthChangedArgs> OnDeath;

        public float Health
        {
            get
            {
                return health;
            }

            set
            {
                health = value;
                if (health > MaxHealth)
                    health = MaxHealth;
                else if (health < 0)
                    health = 0;
            }
        }

        public float MaxHealth
        {
            get
            {
                return maxHealth;
            }

            set
            {
                maxHealth = value;
                if (maxHealth <= 0)
                    throw new ArgumentOutOfRangeException("maxHealth", "MaxHealth must be greater then zero.");
            }
        }

        public bool DestroyOnDeath
        {
            get
            {
                return destroyOnDeath;
            }
        }

        public DamageAbleObject(Entity parent, string name) : base(parent, name)
        {
        }

        public DamageAbleObject(Entity parent, string name, float health, float maxHealth, bool destroyOnDeath = true) : base(parent, name)
        {
            if (maxHealth < health)
                throw new ArgumentOutOfRangeException("maxHealth", "MaxHealth must be greater or equal health");

            MaxHealth = maxHealth;
            Health = health;
            this.destroyOnDeath = destroyOnDeath;
        }

        public void ChangeHealth(Entity sender, float value)
        {
            HealthChangingArgs args = new HealthChangingArgs(sender, false);
            OnHealthChanging?.Invoke(this, args);
            if (!args.Cancel)
            {
                Health += value;
                OnHealthChanged?.Invoke(this, new HealthChangedArgs(sender));
                if (health <= 0)
                {
                    OnDeath?.Invoke(this, new HealthChangedArgs(sender));
                    if (destroyOnDeath)
                        parent.Destroy();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
