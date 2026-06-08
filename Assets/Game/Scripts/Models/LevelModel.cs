using UnityEngine;
using Configs;
using Core;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Enums;
using Data;

namespace Models
{
    public class LevelModel
    {
        private readonly List<LevelConfig> _levelsData;
        private readonly List<LevelState> _levelStates;
        private readonly LevelVariablesGenerator _generator;
        private readonly GameSaves _gameSaves;

        private const string SaveKeyPrefix = "Level_State_Key";

        public int CurrentLevel {get; private set;}
        
        public LevelModel(List<LevelConfig> levelsData, GameSaves gameSaves, LevelVariablesGenerator generator)
        {
            _levelsData = levelsData;
            _gameSaves = gameSaves;
            _generator = generator;
            _levelStates = new List<LevelState>();
        }
        
        public void Init()
        {
            LoadStates();
        }

        public void SetCurrentLevel(int levelIndex)
        {
            CurrentLevel = levelIndex;   
        }
        
        public List<LevelConfig> GetLevelsData()
        {
            return _levelsData;
        }

        public List<LevelState> GetLevelStates()
        {
            return _levelStates;
        }

        public LevelState GetState(int levelId)
        {
            var levelState = _levelStates.FirstOrDefault(x => x.LevelId == levelId);

            if (levelState == null)
            {
                Debug.LogError($"LevelState not found: {levelId}");
                return _levelStates[0];
            }

            return levelState;
        }

        public LevelVariables GetVariables(int levelId)
        {
            var state = GetState(levelId);
            var data = GetData(levelId);

            if (state == null || data == null || !state.IsSeedGenerated)
                return null;

            return _generator.Generate(state.Seed, data);
        }

        public LevelConfig GetData(int levelId)
        {
            return _levelsData.Find(d => d.LevelId == levelId);
        }

        public void GenerateSeed(int levelId)
        {
            var state = GetState(levelId);

            state.Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            state.IsSeedGenerated = true;

            SaveState(state);
        }

        public void CompleteLevel(int levelId)
        {
            var state = GetState(levelId);
            
            state.Status = LevelStatusType.Completed;
            state.IsReadyForReplay = true;
            SaveState(state);

            UnlockNextLevel(levelId);
        }

        public void ResetSeed(int levelId)
        {
            var state = GetState(levelId);
            
            if (state == null)
                return;

            state.IsSeedGenerated = false;
            state.IsReadyForReplay = false;
            state.Seed = 0;

            SaveState(state);
        }

        private void UnlockNextLevel(int levelId)
        {
            var nextLevelData = _levelsData.Find(d => d.LevelId == levelId + 1);
            if (nextLevelData == null) return;

            var nextState = GetState(nextLevelData.LevelId);
            
            if (nextState == null || nextState.Status != LevelStatusType.Locked)
                return;

            nextState.Status = LevelStatusType.Unlocked;
            SaveState(nextState);
        }

        private void LoadStates()
        {
            foreach (var levelData in _levelsData)
            {
                var json = _gameSaves.ReadData<string>(SaveKeyPrefix + levelData.LevelId);

                if (!string.IsNullOrEmpty(json))
                {
                    var state = JsonUtility.FromJson<LevelState>(json);
                    _levelStates.Add(state);
                }
                else
                {
                    var newState = new LevelState
                    {
                        LevelId = levelData.LevelId,
                        Status = levelData.LevelId == 0 ? LevelStatusType.Unlocked : LevelStatusType.Locked,
                        Seed = 0,
                        IsSeedGenerated = false
                    };
                    _levelStates.Add(newState);
                    SaveState(newState);
                }
            }
        }

        private void SaveState(LevelState state)
        {
            var json = JsonUtility.ToJson(state);
            _gameSaves.WriteData(SaveKeyPrefix + state.LevelId, json);
        }
    }
}
