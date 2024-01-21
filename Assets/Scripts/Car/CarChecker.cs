using System.Linq;
using UnityEngine;
using Controllers;

namespace Car
{
    public class CarChecker : MonoBehaviour
    {
        [SerializeField] private string[] targetTags;
        public CarShootController CarShoot { get; set; }

        private void OnTriggerStay(Collider other)
        {
            if (!ShouldProcessCollider(other)) return;

            var healthController = other.gameObject.GetComponent<HealthController>();
            if (healthController != null && IsTargetTag(other.gameObject.tag))
            {
                CarShoot.target = healthController;
            }
        }

        private bool ShouldProcessCollider(Collider collider)
        {
            // Check if the CarShootController is assigned and if it already has a target.
            return CarShoot != null && CarShoot.target == null;
        }

        private bool IsTargetTag(string tag)
        {
            // Check if the tag of the GameObject is one of the target tags.
            return targetTags.Contains(tag);
        }
    }
}
