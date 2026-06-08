using System;
using UnityEngine.InputSystem;
using Interfaces;

namespace Screens
{
    public class LoseScreen : BaseScreen,ITickable
    {
        private Action _onRestart;

        public void Init(Action onRestart)
        {
            _onRestart = onRestart;
        }

        public void Tick()
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                _onRestart?.Invoke();
            }
        }
    }
}
