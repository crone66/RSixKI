using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RSixKI
{
    public class Weapon : Equipment
    {
        public enum WeaponSlot
        {
            None,
            Primary,
            Secondary,
            FragGrenade,
            FlashBang
        }

        public enum AmmoTyp
        {
            None,
            Clip,
            Shell,
        }

        public enum FireState
        {
            None,
            OK,
            FirerateDelay,
            EmptyMag,
            OutOfAmmo,
            Reloading
        }

        private AmmoTyp ammoTyp;
        private WeaponSlot slot;

        private int maxAmmoCount;
        private int ammoCount;

        private int maxMagSize;
        private int ammoInMag;

        private bool reloading;
        private float reloadTime;
        private float elapsedReloadTime;

        private float fireDelay;
        private float elapsedFireDelay;
        private int mobility;
        private bool suspressed;
        
        private Projectile projectile;
        private Entity owner;
        public Entity Owner
        {
            get
            {
                return owner;
            }
        }

        public bool SuspressAble
        {
            get
            {
                if (projectile != null)
                    return projectile.CanSuspressed;

                return false;
            }
        }

        public int Mobility
        {
            get
            {
                return mobility;
            }
        }

        public bool Suspressed
        {
            get
            {
                return suspressed;
            }

            set
            {
                if(!value || (projectile != null && projectile.CanSuspressed))
                    suspressed = value;
            }
        }

        public int AmmoInMag
        {
            get { return ammoInMag; }
        }

        public int MaxMagSize
        {
            get { return maxMagSize; }
        }

        public bool Reloading
        {
            get { return reloading; }
        }

        public WeaponSlot Slot
        {
            get { return slot; }
        }

        private Noise noise;
        private Noise suspressedNoise;

        public Weapon(string name, Vector2 position, EquipmentType equipType, float fireratePerMinute, Projectile projectile, AmmoTyp ammoTyp, WeaponSlot slot, int maxAmmoCount, int maxMagSize, float reloadTime, int mobility, int count = 1, bool isActiv = true, int teamId = -1) : base(name, position, equipType, count, isActiv, teamId)
        {
            fireDelay = 1000f / (fireratePerMinute / 60f);
            this.projectile = projectile;
            this.ammoTyp = ammoTyp;
            this.maxMagSize = maxMagSize;
            this.maxAmmoCount = maxAmmoCount;
            ammoCount = maxAmmoCount;
            ammoInMag = maxMagSize;
            this.reloadTime = reloadTime;
            this.mobility = mobility;
            this.slot = slot;
            suspressed = false;
        }

        public Weapon(string name, Vector2 position, IEnumerable<EntityComponent> components, EquipmentType equipType, float fireratePerMinute, Projectile projectile, AmmoTyp ammoTyp, WeaponSlot slot, int maxAmmoCount, int maxMagSize, float reloadTime, int mobility, int count = 1, bool isActiv = true, int teamId = -1) : base(name, position, components, equipType, count, isActiv, teamId)
        {
            fireDelay = 1000f / (fireratePerMinute / 60f);
            this.projectile = projectile;
            this.ammoTyp = ammoTyp;
            this.maxMagSize = maxMagSize;
            this.maxAmmoCount = maxAmmoCount;
            ammoCount = maxAmmoCount;
            ammoInMag = maxMagSize;
            this.reloadTime = reloadTime;
            this.mobility = mobility;
            this.slot = slot;
            suspressed = false;
        }

        public void Init(Entity owner, int teamId)
        {
            this.teamId = teamId;
            this.owner = owner;
            noise = new Noise(400, 400, "Noise", Position, false, false, teamId);
            suspressedNoise = new Noise(100, 100, "Noise", Position, false, false, teamId);
            ammoCount = 10000;
        }

        public override void Update(GameTime gameTime)
        {
            if(reloading)
            {
                elapsedReloadTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (elapsedReloadTime <= 0)
                    DoReload();
            }
            else
            {
                if (elapsedFireDelay > 0)
                    elapsedFireDelay -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            base.Update(gameTime);
        }

        private void DoReload()
        {
            if (ammoTyp == AmmoTyp.Clip)
            {
                int required = maxMagSize - ammoInMag;
                if (required <= ammoCount)
                {
                    ammoInMag = maxMagSize;
                    ammoCount -= required;
                }
                else
                {
                    ammoInMag += ammoCount;
                    ammoCount = 0;
                   
                }
                reloading = false;
            }
            else if (ammoTyp == AmmoTyp.Shell)
            {
                if (ammoCount > 0 && ammoInMag < maxMagSize)
                {
                    ammoCount--;
                    ammoInMag++;
                }
                else
                {
                    reloading = false;
                }
            }
            else
            {             
                throw new ArgumentOutOfRangeException("ammoTyp", "Invalid ammoTye \"None\"");
            }
        }

        public void Reload()
        {
            if (!reloading && ammoInMag < maxMagSize && ammoCount > 0)
            {
                elapsedReloadTime = reloadTime;
                reloading = true;
            }
        }

        public FireState Fire(Vector2 position, Vector2 direction, Entity possibleVictim = null)
        {
            if (projectile == null)
                return FireState.None;

            Position = position;
            if(!reloading)
            {
                if (ammoInMag > 0)
                {
                    if (elapsedFireDelay <= 0)
                    {
                        elapsedFireDelay = fireDelay;
                        Projectile newProjectile = projectile.Clone() as Projectile;
                        newProjectile.Init(owner, position, direction, teamId, suspressed, possibleVictim);
                        ammoInMag--;
                        if (suspressed)
                        {
                            Noise newNoise = suspressedNoise.Clone() as Noise;
                            newNoise.Init(position, newNoise.Width, newNoise.Height, teamId);
                        }
                        else
                        {
                            Noise newNoise = noise.Clone() as Noise;
                            newNoise.Init(position, noise.Width, noise.Height, teamId);
                        }

                        return FireState.OK;
                    }
                    else
                    {
                        return FireState.FirerateDelay;
                    }
                }
                else
                {
                    if (ammoCount <= 0)
                        return FireState.OutOfAmmo;
                    else
                        return FireState.EmptyMag;
                }
            }
            return FireState.Reloading;
        }

        public void AbortReload()
        {
            reloading = false;
            elapsedReloadTime = reloadTime;
        }

        public override object Clone()
        {
            return new Weapon(name, Position, equipType, fireDelay * 60 * 1000, projectile, ammoTyp, slot, maxAmmoCount, maxMagSize, reloadTime, mobility, count);
        }
    }
}
