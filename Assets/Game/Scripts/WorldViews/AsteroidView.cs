using UnityEngine;

namespace WorldViews
{
    public class AsteroidView : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D circleCollider;

        public void Activate(Vector3 position)
        {
            transform.position = position;
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}