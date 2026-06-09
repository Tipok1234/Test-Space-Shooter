using UnityEngine;
using Views;

namespace Configs
{
    [CreateAssetMenu(fileName = "PrefabConfig", menuName = "PrefabConfig")]
    public class PrefabConfig : ScriptableObject
    {
        public ShipView ShipPrefab => shipPrefab;
        public BulletView BulletPrefab => bulletPrefab;
        public AsteroidView SmallAsteroidPrefab => smallAsteroidPrefab;
        public AsteroidView MediumAsteroidPrefab => mediumAsteroidPrefab;
        public AsteroidView LargeAsteroidPrefab => largeAsteroidPrefab;
        
        [SerializeField] private ShipView shipPrefab;
        [SerializeField] private BulletView bulletPrefab;
        [SerializeField] private AsteroidView smallAsteroidPrefab;
        [SerializeField] private AsteroidView mediumAsteroidPrefab;
        [SerializeField] private AsteroidView largeAsteroidPrefab;
    }
}
