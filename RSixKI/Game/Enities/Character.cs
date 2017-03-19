using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RSixKI
{
    public class Character : CollidAble
    {
        protected Vector2 direction;
        private float speed;
        private float rotationSpeed;
        private float rotationDirection;

        public Weapon primaryWeapon;
        public Weapon secondaryWeapon;
        public Weapon flashbang;
        public Weapon fragGrenade;
        public Gadget gadget;

        public Weapon inHand;
        

        public Character(float speed, float rotationSpeed, bool blocking, Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(blocking, texture, layer, rotation, scale, color, name, position, isVisable, isActiv, teamId)
        {
            this.speed = speed;
            this.rotationSpeed = rotationSpeed;
            direction = Vector2.Zero;
            OnCollisionEnter += Character_OnCollisionEnter;
        }

        public Character(float speed, float rotationSpeed, bool blocking, Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, IEnumerable<EntityComponent> components, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(blocking, texture, layer, rotation, scale, color, name, position, components, isVisable, isActiv, teamId)
        {
            this.speed = speed;
            this.rotationSpeed = rotationSpeed;
            direction = Vector2.Zero;
            OnCollisionEnter += Character_OnCollisionEnter;
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (direction != Vector2.Zero)
            {
                Position += new Vector2(direction.X * speed * deltaTime, 0);
                direction.X = 0;
                Position += new Vector2(0, direction.Y * speed * deltaTime);
            }

            if (rotationDirection != 0)
            {
                float multiplier = 1f;
                if (inHand != null)
                    multiplier = inHand.Mobility / 100f;

                Rotation += rotationDirection * rotationSpeed * deltaTime * multiplier;
            }

            rotationDirection = 0;
            direction = Vector2.Zero;
            base.Update(gameTime);
        }

        protected void OnMoveUp()
        {
            direction.Y = -1;
        }

        protected void OnMoveDown()
        {
            direction.Y = 1;
        }
        
        protected void OnMoveLeft()
        {
            direction.X = -1;
        }

        protected void OnMoveRight()
        {
            direction.X = 1;
        }

        protected void RotateLeft()
        {
            rotationDirection = -1;   
        }

        protected void RotateRight()
        {
            rotationDirection = 1;
        }

        protected void Reload()
        {
            if(inHand != null)
                inHand.Reload();
        }

        protected Weapon.FireState Attack(Entity possibleVictim = null)
        {
            if(inHand != null)
            {
                Vector2 aimDirection = Helpers.ConvertToDirection(Center, Rotation);
                //Calculate aimDirection
                return inHand.Fire(Center + (aimDirection * CollisionBox.Width / 2), aimDirection, possibleVictim);
            }
            return Weapon.FireState.None;
        }

        protected bool SwitchWeapon(Weapon.WeaponSlot slot)
        {
            Weapon preInHand = inHand;
            bool success = false;
            switch(slot)
            {
                case Weapon.WeaponSlot.Primary:
                    if (primaryWeapon != null)
                    {
                        inHand = primaryWeapon;
                        success = true;
                    }
                    break;
                case Weapon.WeaponSlot.Secondary:
                    if(secondaryWeapon != null)
                    {
                        inHand = secondaryWeapon;
                        success = true;
                    }
                    break;
                case Weapon.WeaponSlot.FragGrenade:
                    if(fragGrenade != null)
                    {
                        inHand = fragGrenade;
                        success = true;
                    }
                    break;
                case Weapon.WeaponSlot.FlashBang:
                    if(flashbang != null)
                    {
                        inHand = flashbang;
                        success = true;
                    }
                    break;
                case Weapon.WeaponSlot.None:
                    success = false;
                    break;
            }
            
            if (preInHand != null && preInHand != inHand && success)
                preInHand.AbortReload();

            return success;
        }

        public void PickupWeapon(Weapon weapon)
        {
            PickupWeapon(weapon, weapon.Slot);
        }

        public void PickupWeapon(Weapon weapon, Weapon.WeaponSlot slot)
        {
            if (weapon == null || slot == Weapon.WeaponSlot.None)
                return;

            weapon.Init(this, teamId);
            switch (slot)
            {
                case Weapon.WeaponSlot.Primary:
                    if (inHand == primaryWeapon)
                        inHand = weapon;
                    primaryWeapon = weapon;
                    break;
                case Weapon.WeaponSlot.Secondary:
                    if (inHand == secondaryWeapon)
                        inHand = weapon;
                    secondaryWeapon = weapon;
                    break;
                case Weapon.WeaponSlot.FragGrenade:
                    if (inHand == fragGrenade)
                        inHand = weapon;
                    fragGrenade = weapon;
                    break;
                case Weapon.WeaponSlot.FlashBang:
                    if (inHand == flashbang)
                        inHand = weapon;
                    flashbang = weapon;
                    break;
            }
        }

        protected void UseGadget()
        {
            if(gadget != null)
            {
                throw new NotImplementedException();
            }
        }

        private void Character_OnCollisionEnter(object sender, CollisionArgs e)
        {
            if(direction != Vector2.Zero)
            {
                Vector2 newPos = Position;

                if (direction.X != 0)
                {
                    if (CollisionBox.X < e.CollisionObject.CollisionBox.X + e.CollisionObject.CollisionBox.Width && CollisionBox.X + CollisionBox.Width > e.CollisionObject.CollisionBox.X)
                    {
                        if (direction.X < 0)
                            newPos = new Vector2(e.CollisionObject.CollisionBox.X + e.CollisionObject.CollisionBox.Width, Position.Y);
                        else
                            newPos = new Vector2(e.CollisionObject.CollisionBox.X - CollisionBox.Width, Position.Y);
                    }
                }
                else if (direction.Y != 0)
                {
                    if (CollisionBox.Y < e.CollisionObject.CollisionBox.Y + e.CollisionObject.CollisionBox.Height && CollisionBox.Y + CollisionBox.Height > e.CollisionObject.CollisionBox.Y)
                    {
                        if (direction.Y < 0)
                            newPos = new Vector2(Position.X, e.CollisionObject.CollisionBox.Y + e.CollisionObject.CollisionBox.Height);
                        else
                            newPos = new Vector2(Position.X, e.CollisionObject.CollisionBox.Y - CollisionBox.Height);
                    }
                }

                if (Position != newPos)
                    Position = newPos;
            }
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
