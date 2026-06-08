using Config;
using Configs;
using Controllers;
using Managers;
using Models;
using UnityEngine;

namespace Core
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private LevelsConfig levelsConfig;
        [SerializeField] private PrefabConfig prefabConfig;
        [SerializeField] private ShipConfig shipConfig;
        [SerializeField] private Ticker ticker;
        [SerializeField] private UIManager uiManager;

        [SerializeField] private Transform asteroidsParent;
        [SerializeField] private Transform bulletsParent;
        
        protected override void Configure(Container builder)
        {
            builder.RegisterSingletonInstance<UIManager>(uiManager);
            
            uiManager.Init();
            
            RegisterServices(builder);
            RegisterModels(builder);
            RegisterControllers(builder);
        
            Init(builder);
        }

        private void RegisterServices(Container builder)
        {
            builder.Register<SaveService, SaveService>(Lifetime.Singleton);
            builder.Register<LevelVariablesGenerator, LevelVariablesGenerator>(Lifetime.Singleton);
            builder.RegisterSingletonInstance<Ticker>(ticker);
            builder.Register<GameManager, GameManager>(Lifetime.Singleton);
        }

        private void RegisterModels(Container builder)
        {
            builder.RegisterSingletonInstance<ShipConfig>(shipConfig);
            builder.Register<ShipModel, ShipModel>(Lifetime.Singleton);

            builder.Register<LevelModel>(c => 
            {
                var levelModel = new LevelModel(
                    levelsConfig.LevelsData,                 
                    c.Resolve<SaveService>(),                
                    c.Resolve<LevelVariablesGenerator>()
                );
                levelModel.Init(); 
                return levelModel;
            }, Lifetime.Singleton);
        }

        private void RegisterControllers(Container builder)
        {
            builder.RegisterSingletonInstance<PrefabConfig>(prefabConfig);
    
            builder.Register<AsteroidPool>(c => new AsteroidPool(
                prefabConfig.SmallAsteroidPrefab,
                prefabConfig.MediumAsteroidPrefab,
                prefabConfig.LargeAsteroidPrefab,
                asteroidsParent
            ), Lifetime.Singleton);
    
            builder.Register<BulletPool>(c => new BulletPool(
                prefabConfig.BulletPrefab, 
                bulletsParent
            ), Lifetime.Singleton);

            builder.Register<BulletController, BulletController>(Lifetime.Singleton);
            builder.Register<AsteroidController, AsteroidController>(Lifetime.Singleton);
            builder.Register<ShipController>(c => new ShipController(
                c.Resolve<ShipModel>(),
                c.Resolve<BulletController>(),
                prefabConfig.ShipPrefab, 
                shipConfig 
            ), Lifetime.Singleton);
            builder.Register<LevelScreenController, LevelScreenController>(Lifetime.Singleton);
            builder.Register<MapScreenController, MapScreenController>(Lifetime.Singleton);
            builder.Register<GameController, GameController>(Lifetime.Singleton);
        }

        private void Init(Container builder)
        {
            var mapController = builder.Resolve<MapScreenController>();
            mapController.Init();
            mapController.Show();

            var levelScreenController = builder.Resolve<LevelScreenController>();
            levelScreenController.Init();
            
            builder.Resolve<GameController>();
        }
    }
}
