using System.Collections.Generic;
using UnityEngine;
using Controllers;
using DG.Tweening;

namespace Car
{
    public class CarShootController : MonoBehaviour
    {
        [SerializeField] private BulletController bullet;
        [SerializeField] private float speedBullet;
        [SerializeField] private float bulletDamage;
        [SerializeField] private float delayShoot;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private Transform gunModel;
        [SerializeField] private string[] targetTags;

        private float _delayCurrentShoot;
        private List<BulletController> _bulletPool;
        private Transform _bulletParent;
        public HealthController target { get; set; }
        public bool canShoot { get; set; }

        private void Awake()
        {
            _bulletPool = new List<BulletController>();
            _bulletParent = new GameObject("Bullet Pool").transform;

            var checker = GetComponentInChildren<CarChecker>();
            if (checker)
            {
                checker.carShoot = this;
            }

            canShoot = true;
        }

        private void Update()
        {
            if (target && canShoot)
            {
                gunModel.DOLookAt(target.transform.position, 0.2f);
                Shoot();
            }
        }

        private void Shoot()
        {
            _delayCurrentShoot -= Time.deltaTime;
            if (_delayCurrentShoot > 0) return;
            _delayCurrentShoot = delayShoot;
            
            shootPoint.LookAt(target.transform.position);

            for (int i = 0; i < _bulletPool.Count; i++)
            {
                if (!_bulletPool[i].gameObject.activeInHierarchy)
                {
                    var bul = _bulletPool[i];
                    var bulRigid = bul.GetComponent<Rigidbody>();
                    var bulletTransform = bul.transform;
                    
                    bulletTransform.position = shootPoint.position;
                    bulletTransform.rotation = shootPoint.rotation;
                    bul.gameObject.SetActive(true);
                    bulRigid.velocity = shootPoint.forward * speedBullet;
                    return;
                }
            }

            var bull = Instantiate(bullet, shootPoint.position, shootPoint.rotation, _bulletParent);
            var bullRigid = bull.GetComponent<Rigidbody>();
            
            bull.damage = bulletDamage;
            bull.targetTags = targetTags;
            bullRigid.velocity = shootPoint.forward * speedBullet;
            _bulletPool.Add(bull);
        }

        public void UpgradeBulletDamage(float value)
        {
            bulletDamage += value;
        }
    }
}
