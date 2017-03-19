using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RSixKI
{
    public class Projectile : CollidAble
    {
        private float damage;
        private float suspressedDamage;
        private float damageReductionPerUnit;

        private float speed;
        private float lifeTime;
        private float elapsedLifeTime;
        private Vector2 direction;

        private bool rayCastShot;
        private bool suspressed;

        private Entity owner;
        private bool crossedCover;

        public Entity Owner
        {
            get
            {
                return owner;
            }
        }

        public bool CrossedCover
        {
            get
            {
                return crossedCover;
            }
        }

        public bool CanSuspressed
        {
            get { return suspressedDamage > 0; }
        }

        public Projectile(float damage, float suspressedDamage, float damageReductionPerUnit, float speed, float lifeTime, bool rayCastShot, bool blocking, Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, bool isVisable = false, bool isActiv = false, int teamId = -1) : base(blocking, texture, layer, rotation, scale, color, name, position, isVisable, isActiv, teamId)
        {
            this.damage = damage;
            this.suspressedDamage = suspressedDamage;
            this.damageReductionPerUnit = damageReductionPerUnit;
            this.speed = speed;
            this.rayCastShot = rayCastShot;
            this.lifeTime = lifeTime;
            OnTriggerEnter += Projectile_OnTriggerEnter;
        }

        public Projectile(float damage, float suspressedDamage, float damageReductionPerUnit, float speed, float lifeTime, bool rayCastShot, bool blocking, Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, IEnumerable<EntityComponent> components, bool isVisable = false, bool isActiv = false, int teamId = -1) : base(blocking, texture, layer, rotation, scale, color, name, position, components, isVisable, isActiv, teamId)
        {
            this.damage = damage;
            this.suspressedDamage = suspressedDamage;
            this.damageReductionPerUnit = damageReductionPerUnit;
            this.speed = speed;
            this.rayCastShot = rayCastShot;
            this.lifeTime = lifeTime;
            OnTriggerEnter += Projectile_OnTriggerEnter;
        }

        public void Init(Entity owner, Vector2 spawnPosition, Vector2 direction, int teamId, bool suspressed, Entity possibleVictim =null)
        {
            this.owner = owner;
            this.suspressed = suspressed && suspressedDamage > 0;
            Position = spawnPosition - new Vector2(texture.Width / 2, texture.Height / 2);
            this.direction = direction;
            this.teamId = teamId;
            elapsedLifeTime = 0;
            if (possibleVictim != null && rayCastShot)
            {
                DamageAbleObject dmg = possibleVictim.GetComponent<DamageAbleObject>();
                if(dmg != null)
                    dmg.ChangeHealth(this, suspressed ? -suspressedDamage : -damage);
            }
        }

        public override void Update(GameTime gameTime)
        {
            elapsedLifeTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedLifeTime >= lifeTime)
            {
                Destroy();
            }

            if (direction != Vector2.Zero)
            {
                Position += direction * speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                base.Update(gameTime);
            }
        }

        private void Projectile_OnTriggerEnter(object sender, CollisionArgs e)
        {
            if (e.CollisionObject != Owner && e.CollisionObject.Blocking)
            {
                if (!rayCastShot)
                {
                    DamageAbleObject health = e.CollisionObject.GetComponent<DamageAbleObject>();
                    if (health != null)
                    {
                        health.ChangeHealth(this, suspressed ? -suspressedDamage :  -damage);
                    }

                    if (e.CollisionObject.Name != "Cover")
                        Destroy();
                    else
                        crossedCover = true;
                }
            }
        }

        public override object Clone()
        {
            return new Projectile(damage, suspressedDamage, damageReductionPerUnit, speed, lifeTime, rayCastShot, Blocking, texture, layer, Rotation, scale, color, name, Position, true, true, teamId);
        }
    }
}
