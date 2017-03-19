using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RSixKI
{
    public abstract class Entity : ICloneable
    {
        public static int Count = 0;   

        private Vector2 position;
        protected List<EntityComponent> components;
        protected int teamId;
        protected int id;
        protected string name;
        private bool isActiv;
        
        public event EventHandler<MovedArgs> OnMoved;
        public event EventHandler<MovingArgs> OnMoving;
        public event EventHandler OnSpawned;
        public event EventHandler OnDestroyed;
        public event EventHandler OnActivStateChanged;

        public Vector2 Position
        {
            get { return position; }
            set
            {
                MovingArgs args = new MovingArgs(position, value);
                OnMoving?.Invoke(this, args);
                if (!args.Cancel)
                {
                    position = args.Position;
                    OnMoved?.Invoke(this, new MovedArgs(args.PreviousePosition, position));
                }
            }
        }

        public bool IsActiv
        {
            get
            {
                return isActiv;
            }

            set
            {
                isActiv = value;
                OnActivStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string Name
        {
            get { return name; }
        }

        public int TeamId
        {
            get { return teamId; }
        }

        public Entity(string name, Vector2 position, bool isActiv = true, int teamId = -1)
        {
            this.name = name;
            this.position = position;
            this.isActiv = isActiv;
            this.teamId = teamId;
            id = Count++;
            components = new List<EntityComponent>();

            EntityManager.ActivEntityManager.SignIn(this);

            if (isActiv)
                OnSpawned?.Invoke(this, EventArgs.Empty);
        }

        public Entity(string name, Vector2 position, IEnumerable<EntityComponent> components, bool isActiv = true, int teamId = -1)
        {
            this.name = name;
            this.position = position;
            this.isActiv = isActiv;
            this.teamId = teamId;
            id = Count++;
            this.components = components.ToList();

            EntityManager.ActivEntityManager.SignIn(this);

            if (isActiv)
                OnSpawned?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (EntityComponent item in components)
            {
                item.Update(gameTime);
            }
        }

        public void Destroy()
        {
            IsActiv = false;
            components.Clear();
            EntityManager.ActivEntityManager.SignOut(this);
            OnDestroyed?.Invoke(this, EventArgs.Empty);
        }

        public void Spawn()
        {
            IsActiv = true;
            OnSpawned?.Invoke(this, EventArgs.Empty);
        }

        public T GetComponent<T>() where T : EntityComponent
        {  
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T)
                    return (T)components[i];
            }

            return null;
        }

        public T[] GetComponents<T>() where T : EntityComponent
        {
            List<T> result = new List<T>();
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T)
                    result.Add((T)components[i]);
            }

            if(result.Count > 0)
                return result.ToArray();

            return null;
        }

        public void AddComponent(EntityComponent component)
        {
            if (component != null)
            {
                if (components == null)
                    components = new List<EntityComponent>();

                components.Add(component);
            }
        }

        public abstract object Clone();
    }
}
