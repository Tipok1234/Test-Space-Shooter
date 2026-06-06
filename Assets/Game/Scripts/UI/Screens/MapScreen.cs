using System.Collections.Generic;
using UnityEngine;
using Views;
using System;
using Datas;
using DataUtils;

namespace Screens
{
    public class MapScreen : BaseScreen
    {
        private Action<int> LevelClick;
        
        [SerializeField] private RectTransform content;
        [SerializeField] private LevelView levelViewPrefab;

        private readonly List<LevelView> _levelViews = new List<LevelView>();

        public void Init(List<LevelData> levelsData, Action<int> onLevelClick)
        {
            LevelClick = onLevelClick;
            
            foreach (var levelData in levelsData)
            {
                var levelView = Instantiate(levelViewPrefab, content);
                levelView.Init(levelData.LevelId, LevelClick);
                _levelViews.Add(levelView);
            }
        }

        public void UpdateView(List<LevelState> levelStates)
        {
            foreach (var levelView in _levelViews)
            {
                var state = levelStates.Find(s => s.LevelId == levelView.LevelId);
                if (state != null)
                {
                    levelView.UpdateView(state);
                }
            }
        }
    }
}