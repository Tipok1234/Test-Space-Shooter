using Config;

namespace Models
{ 
    public class ShipModel
    {
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get;private set; }

        public ShipModel(ShipConfig shipConfig)
        {
            MaxHealth = shipConfig.MaxHealth;
            CurrentHealth = MaxHealth;
        }

        public void TakeDamage()
        {
            CurrentHealth--;
        }
        
        public void ResetShipModel()
        {
            CurrentHealth = MaxHealth;
        }
        public bool IsDead => CurrentHealth <= 0;
    }
}
