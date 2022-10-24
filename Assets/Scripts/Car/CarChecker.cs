using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Controllers;

namespace Car
{
    public class CarChecker : MonoBehaviour
    {
        [SerializeField] private string[] targetTags;
        public CarShootController carShoot { get; set; }

        private void OnTriggerStay(Collider other)
        {
            if (!carShoot) return;
            if (carShoot.target) return;
            if (other.gameObject.GetComponent<HealthController>())
            {
                bool find = false;
                for (int i = 0; i < targetTags.Length; i++)
                {
                    if (other.gameObject.CompareTag(targetTags[i]))
                        find = true;
                }

                if (!find) return;

                var target = other.gameObject.GetComponent<HealthController>();
                carShoot.target = target;
            }
        }
    }
}
