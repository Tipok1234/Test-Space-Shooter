using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "ShipConfig", menuName = "ShipConfig")]
    public class ShipConfig : ScriptableObject
    {
        public int MaxHealth => maxHealth;
        public float Speed => speed;
        public float ShipSizeX => shipSizeX;
        public float ShipSizeBottom => shipSizeBottom;
        public float ShipSizeTop => shipSizeTop;
        
        [SerializeField] private int maxHealth;
        [SerializeField] private float speed;
        [SerializeField] private float shipSizeX;
        [SerializeField] private float shipSizeBottom;
        [SerializeField] private float shipSizeTop;
    }
}
