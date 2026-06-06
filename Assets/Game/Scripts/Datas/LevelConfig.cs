using System;
using System.Collections.Generic;
using UnityEngine;

namespace Datas
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public List<LevelData> LevelsData = new List<LevelData>();
    }
    
    [Serializable]
    public class LevelData 
    {
        public int LevelId;

        public int MinAsteroidCount;
        public int MaxAsteroidCount;
        
        public float MinAsteroidSpeed;
        public float MaxAsteroidSpeed;
    }
}
