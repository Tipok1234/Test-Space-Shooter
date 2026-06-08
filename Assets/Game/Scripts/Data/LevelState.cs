using System;
using Enums;

namespace Data
{
    [Serializable]
    public class LevelState
    {
        public int LevelId;
        public int Seed;
        
        public LevelStatusType Status;
        
        public bool IsSeedGenerated;
        public bool IsReadyForReplay;
    }
}
