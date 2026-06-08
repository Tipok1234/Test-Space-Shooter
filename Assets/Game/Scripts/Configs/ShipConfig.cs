using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "ShipConfig", menuName = "ShipConfig")]
    public class ShipConfig : ScriptableObject
    {
        public int MaxHealth;
        
        public float Speed;

        public float ShipSizeX;
        public float ShipSizeBottom;
        public float ShipSizeTop;
    }
}
