using Config;
using UnityEngine;
using Configs;
using Core;
using Managers;
using Models;
using Controllers;
using UnityEngine.Serialization;

namespace Core
{
    [DefaultExecutionOrder(10)]
    public class EntryPoint : MonoBehaviour
    {
        [FormerlySerializedAs("levelConfig")] [SerializeField] private LevelsConfig levelsConfig;
        [SerializeField] private PrefabConfig prefabConfig;
        [SerializeField] private ShipConfig shipConfig;
        [SerializeField] private Ticker ticker;
        [SerializeField] private UIManager uiManager;
        
        [SerializeField] private Transform asteroidsParent;
        [SerializeField] private Transform bulletsParent;

        private DIContainer _container;

        private void Awake()
        {
            _container = new DIContainer();
        
            uiManager.Init();
            
            RegisterServices();
            RegisterModels();
            RegisterControllers();
        
            Init();
        }

        private void RegisterServices()
        {
            _container.Register(new GameSaves());
            _container.Register(new LevelVariablesGenerator());
            _container.Register(ticker);
        }

        private void RegisterModels()
        {
            var levelModel = new LevelModel(
                levelsConfig.LevelsData,
                _container.Resolve<GameSaves>(),
                _container.Resolve<LevelVariablesGenerator>()
            );
    
            levelModel.Init(); 
    
            _container.Register(levelModel);
            _container.Register(new ShipModel(shipConfig));
            _container.Register(new GameManager());
        }

        private void RegisterControllers()
        {
            _container.Register(new AsteroidPool(
                prefabConfig.SmallAsteroidPrefab,
                prefabConfig.MediumAsteroidPrefab,
                prefabConfig.LargeAsteroidPrefab,
                asteroidsParent));
            
            _container.Register(new BulletPool(prefabConfig.BulletPrefab, bulletsParent));
            _container.Register(new BulletController(_container.Resolve<BulletPool>()));
            
            _container.Register(new AsteroidController(_container.Resolve<AsteroidPool>(),_container.Resolve<BulletController>()));
            
            _container.Register(new ShipController(
                _container.Resolve<ShipModel>(),
                _container.Resolve<BulletController>(),
                prefabConfig.ShipPrefab, shipConfig
            ));
            
            _container.Register(new LevelScreenController(
                _container.Resolve<LevelModel>(),
                uiManager,
                _container.Resolve<GameManager>()
            ));
            
            _container.Register(new MapScreenController(
                uiManager,
                _container.Resolve<LevelModel>(),
                _container.Resolve<GameManager>()
            ));
            
            _container.Register(new GameScreenController(
                uiManager,
                _container.Resolve<Ticker>(),
                _container.Resolve<LevelModel>(),
                _container.Resolve<GameManager>(),
                _container.Resolve<BulletController>(),
                _container.Resolve<AsteroidController>(),
                _container.Resolve<ShipController>()
            ));
        }

        private void Init()
        {
            var mapController = _container.Resolve<MapScreenController>();
            mapController.Init();
            mapController.Show();

            var levelScreenController = _container.Resolve<LevelScreenController>();
            levelScreenController.Init();
        }
    }
}
