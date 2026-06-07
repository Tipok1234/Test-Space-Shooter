using System.Collections.Generic;
using Screens;
using UnityEngine;
using Datas;
using DataUtils;
using Managers;
using Models;
using Controllers;
using WorldViews;

namespace Core
{
    [DefaultExecutionOrder(10)]
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private LevelConfig levelConfig;
        [SerializeField] private ShipView shipPrefab;
        [SerializeField] private BulletView bulletPrefab;
        [SerializeField] private Ticker ticker;

        private DIContainer _container;

        private void Awake()
        {
            _container = new DIContainer();
        
            UIManager.Instance.Init();
            
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
                levelConfig.LevelsData,
                _container.Resolve<GameSaves>(),
                _container.Resolve<LevelVariablesGenerator>()
            );
    
            levelModel.Init(); 
    
            _container.Register(levelModel);
            _container.Register(new ShipModel());
        }

        private void RegisterControllers()
        {
            var mapScreen = UIManager.Instance.GetScreen<MapScreen>();
            var levelScreen = UIManager.Instance.GetScreen<LevelScreen>();
            var gameScreen = UIManager.Instance.GetScreen<GameScreen>();

            _container.Register(new MapScreenController(
                mapScreen,
                _container.Resolve<LevelModel>(),
                OnLevelClick
            ));

            _container.Register(new LevelScreenController(
                levelScreen,
                _container.Resolve<LevelModel>(),
                OnPlay
            ));
            
            _container.Register(new GameScreenController(
                gameScreen,
                _container.Resolve<ShipModel>(),
                shipPrefab,
                bulletPrefab,
                _container.Resolve<Ticker>()
            ));
            
            _container.Register(new GameScreenController(
                gameScreen,
                _container.Resolve<ShipModel>(),
                shipPrefab,
                bulletPrefab,
                _container.Resolve<Ticker>()
            ));
        }
        
        private void OnPlay(int levelId)
        {
            _container.Resolve<MapScreenController>().Hide();
            _container.Resolve<LevelScreenController>().Hide();
            _container.Resolve<GameScreenController>().Show(levelId);
        }
        
        private void OnLevelClick(int levelId)
        {
            _container.Resolve<LevelScreenController>().Show(levelId);
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
