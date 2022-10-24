using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Controllers
{
    public class HealthController : MonoBehaviour
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private Image healthBar;

        private float _currentHealth;
        private bool _shield;
        public UnityAction onDead;
        
        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        public void TakeDamage(float value)
        {
            if (_shield) return;
            
            _currentHealth -= value;

            UpdateHealthBar();

            if (_currentHealth <= 0)
            {
                Dead();
            }
        }

        public void AddHealth(float value)
        {
            _currentHealth += value;
            UpdateHealthBar();
        }

        public void UpgradeMaxHealth(float value)
        {
            maxHealth += value;
            _currentHealth = maxHealth;
        }

        public void ActiveShield()
        {
            _shield = true;
            Invoke(nameof(DiactiveShield), 4);
        }

        public void DiactiveShield()
        {
            _shield = false;
        }

        private void UpdateHealthBar() => healthBar.fillAmount = _currentHealth / maxHealth;

        private void Dead()
        {
            onDead?.Invoke();
            Destroy(this);
        }
    }
}
