using Models;
using Screens;
using UnityEngine;
using System;

namespace Controllers
{
    public class MapScreenController 
    {
        private readonly MapScreen _mapScreen;
        private readonly LevelModel _levelModel;
        private readonly Action<int> _onLevelClick;

        public MapScreenController(MapScreen mapScreen, LevelModel levelModel, Action<int> onLevelClick)
        {
            _mapScreen = mapScreen;
            _levelModel = levelModel;
            _onLevelClick = onLevelClick;
        }

        public void Init()
        {
            _mapScreen.Init(_levelModel.GetLevelsData(), OnLevelClick);
        }

        public void Show()
        {
            _mapScreen.UpdateView(_levelModel.GetLevelStates());
            _mapScreen.OpenScreen();
        }

        public void Hide()
        {
            _mapScreen.CloseScreen();
        }

        private void OnLevelClick(int levelId)
        {
            Debug.Log($"Level {levelId} clicked");
            _onLevelClick?.Invoke(levelId);
        }
    }
}
