
    using UnityEngine;

    public class HealthPowerUp : PowerUp
    {
        public int health;
        protected override void ApplyPowerUp(Collider2D other)
        {
            var component = other.GetComponent<Player>();
            if (component != null)
            {
                component.Heal(health);
            }
        }
    }