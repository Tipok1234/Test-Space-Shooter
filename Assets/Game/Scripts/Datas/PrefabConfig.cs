using UnityEngine;
using WorldViews;

namespace Datas
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
