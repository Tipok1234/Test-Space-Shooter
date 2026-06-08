using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Interfaces;

namespace Core
{
    public class Ticker : MonoBehaviour
    {
        private readonly List<ITickable> _tickables = new List<ITickable>();

        public void Register(ITickable tickable)
        {
            if (!_tickables.Contains(tickable))
            {
                _tickables.Add(tickable);
            }
        }

        public void Unregister(ITickable tickable)
        {
            _tickables.Remove(tickable);
        }

        private void Update()
        {
            foreach (var tickable in _tickables.ToList())
            {
                tickable.Tick();
            }
        }
    }
}