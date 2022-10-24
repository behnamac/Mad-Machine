using Controllers;
using Dreamteck.Splines;
using Levels;
using UnityEngine;

namespace Car.Player
{
    public class PlayerMoveController : CarController
    {
        [SerializeField] private float speedHorizontal;
        [SerializeField] private float maxHorizontalMove;
        [SerializeField] private Joystick joystickCar;
        [SerializeField] private Transform finishLine;

        private CarShootController _carShoot;
        private Rigidbody _rigidbody;
        protected override void Awake()
        {
            base.Awake();
            SpeedHorizontal = speedHorizontal;
            MaxHorizontalMove = maxHorizontalMove;
            JoystickCar = joystickCar;

            _carShoot = GetComponent<CarShootController>();
            _rigidbody = GetComponent<Rigidbody>();
            var health = GetComponent<HealthController>();
            if (health)
            {
                health.onDead += OnDead;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!CanMove) return;
            if (_carShoot && _carShoot.target)
            {
                follower.followSpeed = _carShoot.target.GetComponent<SplineFollower>().followSpeed;
            }
            else
            {
                follower.followSpeed = speedForward;
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
            if (health)
            {
                health.onDead -= OnDead;
            }
            
            var particle =
                ParticleManager.PlayParticle("Explode", transform.position + Vector3.up * 1.9f, Quaternion.identity);
            Destroy(particle.gameObject, 3);

            if (_carShoot)
                _carShoot.canShoot = false;

            follower.enabled = false;
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.velocity = new Vector3(Random.Range(-10, 10), Random.Range(10, 20), Random.Range(-5, 5));
            _rigidbody.angularVelocity = new Vector3(0, 0, Random.Range(-10, 10));
            
            LevelManager.Instance.LevelFail();
        }
    }
}
