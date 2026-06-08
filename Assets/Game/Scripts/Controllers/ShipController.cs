using Interfaces;
using Models;
using UnityEngine;
using WorldViews;
using UnityEngine.InputSystem;
using System;

namespace Controllers
{
    public class ShipController : ITickable
    {
        public ShipView Ship => _shipView;
        public int MaxLives => _shipModel.MaxLives;
        
        public event Action OnDeath;
        public event Action<int> OnHealthChanged;
        public event Action<AsteroidView> OnAsteroidHit;
        public event Action OnDeactivate;
        
        private readonly BulletController _bulletController;
        private readonly ShipView _shipViewPrefab;
        
        private ShipModel _shipModel;
        private ShipView _shipView;

        private const float Speed = 5f;
        private const float ShipSizeX = 0.7f;
        private const float ShipSizeBottom = 0.7f;
        private const float ShipSizeTop = 1f;
        
        private Vector2 _minBounds;
        private Vector2 _maxBounds;

        public ShipController(ShipModel shipModel, BulletController bulletController, ShipView shipView)
        {
            _shipViewPrefab = shipView;
            _shipModel = shipModel;
            _bulletController = bulletController;
            
            CalculateBounds();
        }

        public void SpawnShip()
        {
            if (_shipView == null)
            {
                _shipView = UnityEngine.Object.Instantiate(_shipViewPrefab, Vector2.zero, Quaternion.identity);
                _shipView.OnHitAsteroid += OnHitAsteroid;
            }

            _shipView.ResetShip();
        }

        public void ResetShipModel()
        {
            _shipModel.ResetShipModel();
        }

        public void Tick()
        {
            if (_shipView == null) 
                return;
            
            Move();
            HandleShoot();
        }
        
        public void ResetShip()
        {
            if (_shipView)
            {
                _shipView.ResetShip();
                _shipView.OnHitAsteroid -= OnHitAsteroid;
                _shipView.OnHitAsteroid += OnHitAsteroid;
            }
        }
        
        public void Deactivate()
        {
            _shipView.Deactivate();
            OnDeactivate?.Invoke();
        }
        
        private void OnHitAsteroid(AsteroidView asteroidView)
        {
            OnAsteroidHit?.Invoke(asteroidView);
            _shipModel.TakeDamage();
            OnHealthChanged?.Invoke(_shipModel.CurrentLives);

            if (_shipModel.IsDead)
            {
                OnDeath?.Invoke();
            }
        }
        
        private void HandleShoot()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame && _shipView.gameObject.activeSelf)
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

            Vector3 newPosition = _shipView.transform.position + movement * Speed * Time.deltaTime;

            newPosition.x = Mathf.Clamp(newPosition.x, _minBounds.x, _maxBounds.x);
            newPosition.y = Mathf.Clamp(newPosition.y, _minBounds.y, _maxBounds.y);

            _shipView.SetPosition(newPosition);
        }

        private void CalculateBounds()
        {
            var cam = Camera.main;
            _minBounds = cam.ViewportToWorldPoint(new Vector2(0, 0));
            _maxBounds = cam.ViewportToWorldPoint(new Vector2(1, 1));

            _minBounds.x += ShipSizeX;
            _minBounds.y += ShipSizeBottom;
            _maxBounds.x -= ShipSizeX;
            _maxBounds.y -= ShipSizeTop;
        }
    }
}
