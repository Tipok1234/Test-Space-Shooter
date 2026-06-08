using UnityEngine;
using WorldViews;

namespace Configs
{
    [CreateAssetMenu(fileName = "PrefabConfig", menuName = "PrefabConfig")]
    public class PrefabConfig : ScriptableObject
    {
        public ShipView ShipPrefab;
        public BulletView BulletPrefab;
        public AsteroidView SmallAsteroidPrefab;
        public AsteroidView MediumAsteroidPrefab;
        public AsteroidView LargeAsteroidPrefab;
    }
}
