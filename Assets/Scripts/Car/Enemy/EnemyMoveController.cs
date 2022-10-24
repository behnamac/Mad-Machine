using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

namespace Car.Enemy
{
    public class EnemyMoveController : CarController
    {
        private CarShootController _carShoot;
        private Rigidbody _rigidbody;
        protected override void Awake()
        {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody>();
            _carShoot = GetComponent<CarShootController>();
            var health = GetComponent<HealthController>();
            health.onDead += OnDead;
        }

        private void OnDead()
        {
            var health = GetComponent<HealthController>();
            health.onDead -= OnDead;

            UiController.instance.AddCoin(10);
            
            var particle =
                ParticleManager.PlayParticle("Explode", transform.position + Vector3.up * 1.9f, Quaternion.identity);
            Destroy(particle.gameObject, 3);
            
            follower.enabled = false;
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.velocity = new Vector3(Random.Range(-10, 10), Random.Range(10, 20), Random.Range(-5, 5));
            _rigidbody.angularVelocity = new Vector3(0, 0, Random.Range(-10, 10));
            
            if (_carShoot)
                _carShoot.canShoot = false;
            
            EnemyHolder.Instance.RemoveEnemy();
        }
    }
}
