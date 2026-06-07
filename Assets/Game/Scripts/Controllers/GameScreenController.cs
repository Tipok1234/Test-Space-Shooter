using Core;
using Managers;
using Screens;
using Models;
using UnityEngine;
using WorldViews;

namespace Controllers
{
    public class GameScreenController 
    {
        private readonly GameScreen _gameScreen;
        private readonly ShipModel _shipModel;
        private readonly ShipView _shipPrefab;
        private readonly BulletView _bulletPrefab;
        private readonly Ticker _ticker;

        private ShipView _shipView;
        private ShipController _shipController;
        private BulletController _bulletController;

        public GameScreenController(GameScreen gameScreen, ShipModel shipModel, ShipView shipPrefab, BulletView bulletPrefab, Ticker ticker)
        {
            _gameScreen = gameScreen;
            _shipModel = shipModel;
            _shipPrefab = shipPrefab;
            _bulletPrefab = bulletPrefab;
            _ticker = ticker;
        }

        public void Show(int levelId)
        {
            _gameScreen.OpenScreen();
            SpawnShip();
            SpawnBullets();
            InitShipController();
        }

        public void Hide()
        {
            _gameScreen.CloseScreen();
        }
        
        private void InitShipController()
        {
            _shipController = new ShipController(_shipModel, _shipView, _bulletController);
            _ticker.Register(_shipController);
        }

        private void SpawnShip()
        {
            _shipView = Object.Instantiate(_shipPrefab, _shipModel.SpawnPosition, Quaternion.identity);
            // _shipController = new ShipController(_shipModel, _shipView, _bulletController);
            // _ticker.Register(_shipController);
        }

        private void SpawnBullets()
        {
            var bulletPool = new BulletPool(_bulletPrefab, _shipView.BulletSpawnPoint);
            _bulletController = new BulletController(bulletPool);
            _ticker.Register(_bulletController);
        }
    }
}
