using System;
using Configs;
using Enums;
using Data;

namespace Managers
{
    public class LevelVariablesGenerator
    {
        private static readonly AsteroidType[] AsteroidTypes = 
            (AsteroidType[])Enum.GetValues(typeof(AsteroidType));
        
        public LevelVariables Generate(int seed, LevelConfig levelConfig)
        {
            var random = new Random(seed);
    
            return new LevelVariables(
                asteroidCount: random.Next(levelConfig.MinAsteroidCount, levelConfig.MaxAsteroidCount + 1),
                asteroidSpeed: (float)(random.NextDouble() * (levelConfig.MaxAsteroidSpeed - levelConfig.MinAsteroidSpeed) + levelConfig.MinAsteroidSpeed),
                asteroidType: AsteroidTypes[random.Next(AsteroidTypes.Length)]
            );
        }
    }
}
