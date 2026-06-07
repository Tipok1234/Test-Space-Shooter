using UnityEngine;

namespace WorldViews
{
    public class ShipView : MonoBehaviour
    {
        public Transform BulletSpawnPoint => bulletSpawnPoint;
        
        [SerializeField] private Transform bulletSpawnPoint;
        
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}
