using System;
using System.Collections.Generic;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "LevelsConfig")]
    public class LevelsConfig : ScriptableObject
    {
        public List<LevelConfig> LevelsData = new List<LevelConfig>();
    }
    
    [Serializable]
    public class LevelConfig 
    {
        public int LevelId;

        public int MinAsteroidCount = 1;
        public int MaxAsteroidCount = 1;
        
        public float MinAsteroidSpeed;
        public float MaxAsteroidSpeed;
    }
}
