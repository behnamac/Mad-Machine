using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class BulletController : MonoBehaviour
    {
        public float damage { get; set; }
        public string[] targetTags { get; set; }
        private void Awake()
        {
            Invoke(nameof(Diactive), 3);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<HealthController>())
            {
                bool find = false;
                for (int i = 0; i < targetTags.Length; i++)
                {
                    if (other.gameObject.CompareTag(targetTags[i]))
                        find = true;
                }
                if (!find) return;
                
                var target = other.GetComponent<HealthController>();
                target.TakeDamage(damage);
                Diactive();
            }
        }

        private void Diactive()
        {
            gameObject.SetActive(false);
        }
    }
}
