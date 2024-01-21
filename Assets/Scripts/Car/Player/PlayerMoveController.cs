using Controllers;
using Dreamteck.Splines;
using Levels;
using UnityEngine;

namespace Car.Player
{
    public class PlayerMoveController : CarController
    {
        [SerializeField] private Joystick horizontalInputJoystick; // Updated variable name
        [SerializeField] private Transform finishLine;

        private CarShootController _carShoot;
        private Rigidbody _rigidbody;

        protected override void Awake()
        {
            base.Awake();
            joystickCar = horizontalInputJoystick; // Updated variable assignment

            _carShoot = GetComponent<CarShootController>();
            _rigidbody = GetComponent<Rigidbody>();
            var health = GetComponent<HealthController>();

            if (health != null) // Null check
            {
                health.onDead += OnDead;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!canMove) return;

            if (_carShoot && _carShoot.target)
            {
                Follower.followSpeed = _carShoot.target.GetComponent<SplineFollower>().followSpeed;
            }
            else
            {
                Follower.followSpeed = speedForward;
            }
        }

        protected override void OnLevelComplete(Level level)
        {
            base.OnLevelComplete(level);
            finishLine.gameObject.SetActive(true);
            finishLine.SetParent(null);
        }

        private void OnDead()
        {
            var health = GetComponent<HealthController>();

            if (health != null) // Null check
            {
                health.onDead -= OnDead;
            }

            // Extract the code for handling player death into a separate method
            HandlePlayerDeath();
        }

        private void HandlePlayerDeath()
        {
            var particle =
                ParticleManager.PlayParticle("Explode", transform.position + Vector3.up * 1.9f, Quaternion.identity);
            Destroy(particle.gameObject, 3);

            if (_carShoot)
            {
                _carShoot.canShoot = false;
            }

            Follower.enabled = false;
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.velocity = new Vector3(Random.Range(-10, 10), Random.Range(10, 20), Random.Range(-5, 5));
            _rigidbody.angularVelocity = new Vector3(0, 0, Random.Range(-10, 10));

            LevelManager.Instance.LevelFail();
        }
    }
}
