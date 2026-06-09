using System;
using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "LevelsConfig")]
    public class LevelsConfig : ScriptableObject
    {
        public List<LevelConfig> LevelsData => levelsData;
        
        [SerializeField] private List<LevelConfig> levelsData = new List<LevelConfig>();
    }
    
    [Serializable]
    public class LevelConfig 
    {
        public int LevelId => levelId;
        public int MinAsteroidCount => minAsteroidCount;
        public int MaxAsteroidCount => maxAsteroidCount;
        public float MinAsteroidSpeed => minAsteroidSpeed;
        public float MaxAsteroidSpeed => maxAsteroidSpeed;

        [SerializeField] private int levelId;
        [SerializeField] private int minAsteroidCount;
        [SerializeField] private int maxAsteroidCount;
        
        [SerializeField] private float minAsteroidSpeed;
        [SerializeField] private float maxAsteroidSpeed;
    }
}
