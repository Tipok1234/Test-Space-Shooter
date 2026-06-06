using System;
using Enums;

namespace DataUtils
{
    [Serializable]
    public class LevelState
    {
        public int LevelId;
        public int Seed;
        
        public LevelStatusEnum Status;
        
        public bool IsSeedGenerated;
    }
}
