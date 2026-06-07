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
        public event Action OnDeath;
        public event Action<int> OnHealthChanged;
        public event Action<AsteroidView> OnAsteroidHit;
        
        public event Action OnDeactivate;
        
        private readonly ShipModel _shipModel;
        private readonly ShipView _shipView;
        private readonly BulletController _bulletController;

        private const float Speed = 5f;
        private const float ShipSizeX = 0.7f;
        private const float ShipSizeBottom = 0.7f;
        private const float ShipSizeTop = 1f;
        
        private Vector2 _minBounds;
        private Vector2 _maxBounds;

        public ShipController(ShipModel shipModel, ShipView shipView, BulletController bulletController)
        {
            _shipModel = shipModel;
            _shipView = shipView;
            _bulletController = bulletController;

            _shipView.OnHitAsteroid += OnHitAsteroid;
            CalculateBounds();
        }

        public void Tick()
        {
            Move();
            HandleShoot();
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
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
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
