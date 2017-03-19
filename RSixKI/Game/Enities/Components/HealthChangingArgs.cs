using System;

namespace RSixKI
{
    public class HealthChangingArgs : EventArgs
    {
        public Entity HealthChanger;
        public bool Cancel;

        public HealthChangingArgs(Entity healthChanger, bool cancel)
        {
            HealthChanger = healthChanger;
            Cancel = cancel;
        }
    }
}
