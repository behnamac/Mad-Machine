using Controllers;
using UnityEngine;

namespace Car.Enemy
{
    public class EnemyMoveController : CarController
    {
        private CarShootController _carShoot;
        private Rigidbody _rigidbody;
        private HealthController _healthController;

        [SerializeField] private int coinReward = 10;
        [SerializeField] private float explosionHeight = 1.9f;
        [SerializeField] private Vector3 minVelocity = new Vector3(-10, 10, -5);
        [SerializeField] private Vector3 maxVelocity = new Vector3(10, 20, 5);
        [SerializeField] private Vector3 angularVelocityRange = new Vector3(0, 0, 10);

        protected override void Awake()
        {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody>();
            _carShoot = GetComponent<CarShootController>();
            _healthController = GetComponent<HealthController>();

            // Subscribe to the onDead event
            _healthController.onDead += OnDead;
        }

        private void OnDead()
        {
            // Unsubscribe from the onDead event
            _healthController.onDead -= OnDead;

            // Reward player with coins
            UiController.instance.AddCoin(coinReward);

            // Spawn explosion particles
            var particle = ParticleManager.PlayParticle("Explode", transform.position + Vector3.up * explosionHeight, Quaternion.identity);
            Destroy(particle.gameObject, 3);

            // Enable physics for rigidbody
            Follower.enabled = false;
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;

            // Apply random velocity and angular velocity
            _rigidbody.velocity = new Vector3(Random.Range(minVelocity.x, maxVelocity.x), Random.Range(minVelocity.y, maxVelocity.y), Random.Range(minVelocity.z, maxVelocity.z));
            _rigidbody.angularVelocity = new Vector3(0, 0, Random.Range(-angularVelocityRange.z, angularVelocityRange.z));

            // Disable shooting (if applicable)
            if (_carShoot)
                _carShoot.canShoot = false;

            // Notify the enemy holder that an enemy has been removed
            EnemyHolder.Instance.RemoveEnemy();
        }
    }
}
