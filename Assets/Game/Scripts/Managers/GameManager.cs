using System;
using Enums;
using UnityEngine;

namespace Managers
{
    public class GameManager
    {
        public event Action<GameStateType> GameStateChanged; 
        
        private GameStateType _gameState;

        public void SetState(GameStateType gameState)
        {
            if (_gameState != gameState)
            {
                _gameState = gameState;
                GameStateChanged?.Invoke(_gameState);
            }
        }
    }
}
