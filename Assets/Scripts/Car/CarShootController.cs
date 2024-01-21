using System.Collections.Generic;
using UnityEngine;
using Controllers;
using DG.Tweening;

namespace Car
{
    public class CarShootController : MonoBehaviour
    {
        [SerializeField] private BulletController bulletPrefab;
        [SerializeField] private float bulletSpeed;
        [SerializeField] private float bulletDamage;
        [SerializeField] private float shootingDelay;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private Transform gunModel;
        [SerializeField] private string[] targetTags;

        private float _currentShootingDelay;
        private List<BulletController> _bulletPool;
        private Transform _bulletParent;
        public HealthController target { get; set; }
        public bool canShoot { get; set; }

        private void Awake()
        {
            InitializeBulletPool();
            AssignCheckerToCarShoot();
            canShoot = true;
        }

        private void Update()
        {
            if (target && canShoot)
            {
                AimAtTarget();
                TryShoot();
            }
        }

        private void InitializeBulletPool()
        {
            _bulletPool = new List<BulletController>();
            _bulletParent = new GameObject("Bullet Pool").transform;
        }

        private void AssignCheckerToCarShoot()
        {
            var checker = GetComponentInChildren<CarChecker>();
            if (checker)
            {
                checker.CarShoot = this;
            }
        }

        private void AimAtTarget()
        {
            gunModel.DOLookAt(target.transform.position, 0.2f);
        }

        private void TryShoot()
        {
            _currentShootingDelay -= Time.deltaTime;
            if (_currentShootingDelay <= 0)
            {
                _currentShootingDelay = shootingDelay;
                Shoot();
            }
        }

        private void Shoot()
        {
            shootPoint.LookAt(target.transform.position);
            BulletController bullet = GetOrCreateBullet();
            ActivateBullet(bullet);
        }

        private BulletController GetOrCreateBullet()
        {
            foreach (BulletController existingBullet in _bulletPool)
            {
                if (!existingBullet.gameObject.activeInHierarchy)
                {
                    return existingBullet;
                }
            }

            return CreateNewBullet();
        }

        private BulletController CreateNewBullet()
        {
            BulletController newBullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation, _bulletParent);
            newBullet.damage = bulletDamage;
            newBullet.targetTags = targetTags;
            _bulletPool.Add(newBullet);
            return newBullet;
        }

        private void ActivateBullet(BulletController bullet)
        {
            Transform bulletTransform = bullet.transform;
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

            bulletTransform.position = shootPoint.position;
            bulletTransform.rotation = shootPoint.rotation;
            bullet.gameObject.SetActive(true);
            bulletRigidbody.velocity = shootPoint.forward * bulletSpeed;
        }

        public void UpgradeBulletDamage(float value)
        {
            bulletDamage += value;
        }
    }
}
