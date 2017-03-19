using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RSixKI
{
    public class Player : Character
    {
        private const float padding = 3f;

        private int hostagesGrabbed = 0;
        private int numberOfHostages;
        private bool hostagesRescued;
        protected List<Entity> hostages;

        public bool HostagesRescued
        {
            get { return hostagesRescued; }
        }

        public int HostageGrabbed
        {
            get { return hostagesGrabbed; }
        }

        public Player(float speed, float rotationSpeed, bool blocking, Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(speed, rotationSpeed, blocking, texture, layer, rotation, scale, color, name, position, isVisable, isActiv, teamId)
        {
            OnCollisionEnter += Player_OnCollisionEnter;
            OnTriggerEnter += Player_OnTriggerEnter;
            hostages = new List<Entity>(EntityManager.ActivEntityManager.GetEntitiesByName("Hostage"));
            if (hostages != null)
            {
                numberOfHostages = hostages.Count;
                for (int i = 0; i < hostages.Count; i++)
                {
                    hostages[i].OnDestroyed += Player_OnDestroyed;
                }
            }
        }

        public Player(float speed, float rotationSpeed, bool blocking, Texture2D texture, int layer, float rotation, Vector2 scale, Color color, string name, Vector2 position, IEnumerable<EntityComponent> components, bool isVisable = true, bool isActiv = true, int teamId = -1) : base(speed, rotationSpeed, blocking, texture, layer, rotation, scale, color, name, position, components, isVisable, isActiv, teamId)
        {
            OnCollisionEnter += Player_OnCollisionEnter;
            OnTriggerEnter += Player_OnTriggerEnter;
            hostages = new List<Entity>(EntityManager.ActivEntityManager.GetEntitiesByName("Hostage"));
            if (hostages != null)
            {
                numberOfHostages = hostages.Count;
                for (int i = 0; i < hostages.Count; i++)
                {
                    hostages[i].OnDestroyed += Player_OnDestroyed;
                }
            }
        }

        private void Player_OnDestroyed(object sender, EventArgs e)
        {
            hostages.Remove(sender as Entity);
        }

        public override void Update(GameTime gameTime)
        {      
            KeyboardState state = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            if (state.IsKeyDown(Keys.W))
                OnMoveUp();
            if (state.IsKeyDown(Keys.S))
                OnMoveDown();
            if (state.IsKeyDown(Keys.A))
                OnMoveLeft();
            if (state.IsKeyDown(Keys.D))
                OnMoveRight();
            if (state.IsKeyDown(Keys.Space) || mouseState.LeftButton == ButtonState.Pressed)
                Attack();
            if (state.IsKeyDown(Keys.R))
                Reload();

            
            switch(GetRotationDirection(mouseState))
            {
                case Helpers.RotationDirection.Left:
                    RotateLeft();
                    break;
                case Helpers.RotationDirection.Right:
                    RotateRight();
                    break;
            }

            base.Update(gameTime);
        }

        public void BaseUpdate(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        private Helpers.RotationDirection GetRotationDirection(MouseState state)
        {
            Vector2 mousePosition = state.Position.ToVector2();

            Vector2 direction = Center - mousePosition;
            float targetAngle = Helpers.ConvertToAngle(direction);

            Helpers.RotationDirection rotDirection = Helpers.GetRotationDirection(targetAngle, Rotation, padding);
            if (rotDirection == Helpers.RotationDirection.None)
                Rotation = targetAngle;

            return rotDirection;     
        }

        private void Player_OnCollisionEnter(object sender, CollisionArgs e)
        {
            if(e.CollisionObject is Character)
            {
                if(e.CollisionObject.Name == "Hostage")
                {
                    hostagesGrabbed++;
                    e.CollisionObject.Destroy();
                }
            }
        }

        private void Player_OnTriggerEnter(object sender, CollisionArgs e)
        {
            if(e.CollisionObject is InteractivObject && e.CollisionObject.Name == "RescueZone")
            {
                if(hostagesGrabbed >= numberOfHostages)
                {
                    hostagesRescued = true;
                    Console.WriteLine("Hostages rescued!");
                }
            }
        }
    }
}
