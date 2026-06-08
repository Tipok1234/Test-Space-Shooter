using System;
using Enums;

namespace Data
{
    [Serializable]
    public class LevelVariables
    {
        public int AsteroidCount;
        
        public float AsteroidSpeed;
        
        public AsteroidType AsteroidType;
        
        public LevelVariables(int asteroidCount, float asteroidSpeed, AsteroidType asteroidType)
        {
            AsteroidCount = asteroidCount;
            AsteroidSpeed = asteroidSpeed;
            AsteroidType = asteroidType;
        }
    }
}
