using UnityEngine;
using Views;
using System.Collections.Generic;
using Enums;

namespace Managers
{
    public class AsteroidPool
    {
        private readonly Queue<AsteroidView> _smallPool = new Queue<AsteroidView>();
        private readonly Queue<AsteroidView> _mediumPool = new Queue<AsteroidView>();
        private readonly Queue<AsteroidView> _largePool = new Queue<AsteroidView>();

        private readonly AsteroidView _smallPrefab;
        private readonly AsteroidView _mediumPrefab;
        private readonly AsteroidView _largePrefab;

        private readonly Transform _parent;

        private const int InitialSize = 20;

        public AsteroidPool(AsteroidView smallPrefab, AsteroidView mediumPrefab, AsteroidView largePrefab,
            Transform parent)
        {
            _smallPrefab = smallPrefab;
            _mediumPrefab = mediumPrefab;
            _largePrefab = largePrefab;
            _parent = parent;

            for (int i = 0; i < InitialSize; i++)
            {
                _smallPool.Enqueue(CreateAsteroid(_smallPrefab));
                _mediumPool.Enqueue(CreateAsteroid(_mediumPrefab));
                _largePool.Enqueue(CreateAsteroid(_largePrefab));
            }
        }

        public AsteroidView Get(AsteroidType type, Vector3 position)
        {
            var pool = GetPool(type);
            var asteroid = pool.Count > 0 ? pool.Dequeue() : CreateAsteroid(GetPrefab(type));
            asteroid.Activate(position);
            return asteroid;
        }

        public void Return(AsteroidView asteroid, AsteroidType type)
        {
            asteroid.Deactivate();
            GetPool(type).Enqueue(asteroid);
        }

        private Queue<AsteroidView> GetPool(AsteroidType type)
        {
            return type switch
            {
                AsteroidType.Small => _smallPool,
                AsteroidType.Medium => _mediumPool,
                AsteroidType.Large => _largePool,
                _ => _smallPool
            };
        }

        private AsteroidView GetPrefab(AsteroidType type)
        {
            return type switch
            {
                AsteroidType.Small => _smallPrefab,
                AsteroidType.Medium => _mediumPrefab,
                AsteroidType.Large => _largePrefab,
                _ => _smallPrefab
            };
        }

        private AsteroidView CreateAsteroid(AsteroidView prefab)
        {
            var asteroid = Object.Instantiate(prefab, _parent);
            asteroid.Deactivate();
            return asteroid;
        }
    }
}
