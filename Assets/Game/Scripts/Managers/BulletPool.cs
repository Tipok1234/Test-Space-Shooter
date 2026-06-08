using System.Collections.Generic;
using UnityEngine;
using Views;

namespace Managers
{
    public class BulletPool 
    {
        private readonly BulletView _bulletPrefab;
        private readonly Transform _bulletParent;
        private readonly Queue<BulletView> _pool = new Queue<BulletView>();

        private const int InitialSize = 20;

        public BulletPool(BulletView bulletPrefab, Transform bulletParent)
        {
            _bulletPrefab = bulletPrefab;
            _bulletParent = bulletParent;

            for (int i = 0; i < InitialSize; i++)
            {
                CreateBullet();
            }
        }

        public BulletView Get(Vector3 position)
        {
            var bullet = _pool.Count > 0 ? _pool.Dequeue() : CreateBullet();
            bullet.transform.SetParent(null); 
            bullet.Launch(position);
            return bullet;
        }

        public void Return(BulletView bullet)
        {
            bullet.transform.SetParent(_bulletParent);
            bullet.Deactivate();
            _pool.Enqueue(bullet);
        }

        private BulletView CreateBullet()
        {
            var bullet = Object.Instantiate(_bulletPrefab, _bulletParent);
            bullet.Deactivate();
            return bullet;
        }
    }
}
