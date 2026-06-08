using Data;
using UnityEngine;
using System.Collections.Generic;
using Configs;
using Enums;

namespace Core
{
    public class SaveService
    {
        private const string SaveKeyPrefix = "Level_State_Key";
        
        public void SaveLevelState(LevelState state)
        {
            var json = JsonUtility.ToJson(state);
            WriteData(SaveKeyPrefix + state.LevelId, json);
        }

        public List<LevelState> LoadLevelStates(IEnumerable<LevelConfig> levelsData)
        {
            var levelStates = new List<LevelState>();

            foreach (var levelData in levelsData)
            {
                var json = ReadData<string>(SaveKeyPrefix + levelData.LevelId);

                if (!string.IsNullOrEmpty(json))
                {
                    var state = JsonUtility.FromJson<LevelState>(json);
                    levelStates.Add(state);
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
                    levelStates.Add(newState);
                    SaveLevelState(newState);
                }
            }

            return levelStates;
        }
        
        public void WriteData<T>(string key, T data)
        {  
            if (typeof(T) == typeof(int))
            {
                PlayerPrefs.SetInt(key, (int)(object)data);
            }
            else if (typeof(T) == typeof(float))
            {
                PlayerPrefs.SetFloat(key, (float)(object)data);
            }
            else if (typeof(T) == typeof(string))
            {
                PlayerPrefs.SetString(key, (string)(object)data);
            }
            else if (typeof(T) == typeof(bool))
            {
                PlayerPrefs.SetInt(key, (bool)(object)data ? 1 : 0);
            }
            else
            {
                Debug.LogError($"Wrong data type: Attempted to write type {typeof(T).Name}");
            }

            PlayerPrefs.Save();
        }

        public T ReadData<T>(string key)
        {
            if (typeof(T) == typeof(int))
            {
                return (T)(object)PlayerPrefs.GetInt(key);
            }

            if (typeof(T) == typeof(float))
            {
                return (T)(object)PlayerPrefs.GetFloat(key);
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)PlayerPrefs.GetString(key);
            }

            if (typeof(T) == typeof(bool))
            {
                int intValue = PlayerPrefs.GetInt(key);
                return (T)(object)(intValue != 0);
            }

            Debug.LogError("There are no Saves");
            return default(T);
        }
    }
}
