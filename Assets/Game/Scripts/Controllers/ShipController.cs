using Interfaces;
using Models;
using UnityEngine;
using WorldViews;
using UnityEngine.InputSystem;
using System;
using Config;

namespace Controllers
{
    public class ShipController : ITickable
    {
        public int MaxHealth => _shipModel.MaxHealth;
        public event Action Death;
        public event Action<int> HealthChanged;
        public event Action<AsteroidView> AsteroidHit;
        
        private readonly BulletController _bulletController;
        private readonly ShipView _shipViewPrefab;
        
        private ShipModel _shipModel;
        private ShipView _shipView;
        private ShipConfig _shipConfig;
        
        private Vector2 _minBounds;
        private Vector2 _maxBounds;

        private bool isEnabled;

        public ShipController(ShipModel shipModel, BulletController bulletController, ShipView shipView, ShipConfig shipConfig)
        {
            _shipViewPrefab = shipView;
            _shipModel = shipModel;
            _bulletController = bulletController;
            _shipConfig = shipConfig;
            
            CalculateBounds();
        }

        public void SpawnShip()
        {
            if (_shipView == null)
            {
                _shipView = UnityEngine.Object.Instantiate(_shipViewPrefab, Vector2.zero, Quaternion.identity);
                _shipView.HitAsteroid += OnHitAsteroid;
            }

            _shipView.Activate();
            SetEnabled(true);
        }

        public void Tick()
        {
            if (!isEnabled) 
                return;
            
            Move();
            HandleShoot();
        }
        
        public void Reset()
        {
            _shipModel.ResetShipModel();
            
            if (_shipView)
            {
                _shipView.Activate();
                _shipView.HitAsteroid -= OnHitAsteroid;
                _shipView.HitAsteroid += OnHitAsteroid;
            }

            SetEnabled(false);
        }
        
        public void SetEnabled(bool enabled) => isEnabled = enabled;
        
        public void Deactivate() => _shipView.Deactivate();
        
        private void OnHitAsteroid(AsteroidView asteroidView)
        {
            AsteroidHit?.Invoke(asteroidView);
            _shipModel.TakeDamage();
            HealthChanged?.Invoke(_shipModel.CurrentHealth);

            if (_shipModel.IsDead)
            {
                Death?.Invoke();
            }
        }
        
        private void HandleShoot()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame && isEnabled)
            {
                _bulletController.Shoot(_shipView.BulletSpawnPoint.position);
            }
        }

        private void Move()
        {
            var movement = new Vector3(
                Keyboard.current.leftArrowKey.isPressed ? -1 : Keyboard.current.rightArrowKey.isPressed ? 1 : 0,
                Keyboard.current.downArrowKey.isPressed ? -1 : Keyboard.current.upArrowKey.isPressed ? 1 : 0,
                0
            );

            Vector3 newPosition = _shipView.transform.position + movement * (_shipConfig.Speed * Time.deltaTime);

            newPosition.x = Mathf.Clamp(newPosition.x, _minBounds.x, _maxBounds.x);
            newPosition.y = Mathf.Clamp(newPosition.y, _minBounds.y, _maxBounds.y);

            _shipView.SetPosition(newPosition);
        }

        private void CalculateBounds()
        {
            var cam = Camera.main;
            _minBounds = cam.ViewportToWorldPoint(new Vector2(0, 0));
            _maxBounds = cam.ViewportToWorldPoint(new Vector2(1, 1));

            _minBounds.x += _shipConfig.ShipSizeX;
            _minBounds.y += _shipConfig.ShipSizeBottom;
            _maxBounds.x -= _shipConfig.ShipSizeX;
            _maxBounds.y -= _shipConfig.ShipSizeTop;
        }
    }
}
