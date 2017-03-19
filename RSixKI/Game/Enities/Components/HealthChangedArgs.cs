using System;

namespace RSixKI
{
    public class HealthChangedArgs : EventArgs
    {
        public Entity HealthChanger;

        public HealthChangedArgs(Entity healthChanger)
        {
            HealthChanger = healthChanger;
        }
    }
}
