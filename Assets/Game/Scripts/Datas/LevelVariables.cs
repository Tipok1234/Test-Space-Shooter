using System;
using Enums;

namespace Datas
{
    [Serializable]
    public class LevelVariables
    {
        public int AsteroidCount;
        
        public float AsteroidSpeed;
        
        public AsteroidTypeEnum AsteroidType;
        
        public LevelVariables(int asteroidCount, float asteroidSpeed, AsteroidTypeEnum asteroidType)
        {
            AsteroidCount = asteroidCount;
            AsteroidSpeed = asteroidSpeed;
            AsteroidType = asteroidType;
        }
    }
}
