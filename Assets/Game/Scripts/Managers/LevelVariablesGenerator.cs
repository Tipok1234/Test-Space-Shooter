using System;
using Datas;
using Enums;

namespace Managers
{
    public class LevelVariablesGenerator
    {
        private static readonly AsteroidTypeEnum[] AsteroidTypes = 
            (AsteroidTypeEnum[])Enum.GetValues(typeof(AsteroidTypeEnum));
        
        public LevelVariables Generate(int seed, LevelData levelData)
        {
            var random = new System.Random(seed);
    
            return new LevelVariables(
                asteroidCount: random.Next(levelData.MinAsteroidCount, levelData.MaxAsteroidCount + 1),
                asteroidSpeed: (float)(random.NextDouble() * (levelData.MaxAsteroidSpeed - levelData.MinAsteroidSpeed) + levelData.MinAsteroidSpeed),
                asteroidType: AsteroidTypes[random.Next(AsteroidTypes.Length)]
            );
        }
    }
}
