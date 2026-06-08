using UnityEngine;

namespace Core
{
    public abstract class LifetimeScope : MonoBehaviour
    {
        public Container Container { get; private set; }

        protected virtual void Awake()
        {
            Container = new Container();
            Configure(Container);

            var injectables = FindObjectsOfType<MonoBehaviour>(true);
            foreach (var mb in injectables)
            {
                Container.Inject(mb);
            }
        }

        protected abstract void Configure(Container builder);
    }
}
