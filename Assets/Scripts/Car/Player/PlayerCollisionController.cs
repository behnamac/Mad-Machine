using Controllers;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Car.Player
{
    public class PlayerCollisionController : MonoBehaviour
    {
        [SerializeField] private Transform scoreText;
        [SerializeField] private Transform spawnScoreTextPoint;

        private HealthController _health;
        private PlayerUpgradeController _playerUpgrade;

        // Tags
        private string healthTag = "Health";
        private string damageTag = "Damage";
        private string moneyTag = "Money";
        private string armorTag = "Armor";

        private void Awake()
        {
            _health = GetComponent<HealthController>();
            _playerUpgrade = GetComponent<PlayerUpgradeController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(healthTag))
            {
                HandleHealthCollision(other.gameObject);
            }
            else if (other.CompareTag(damageTag))
            {
                HandleDamageCollision(other.gameObject);
            }
            else if (other.CompareTag(moneyTag))
            {
                HandleMoneyCollision(other.gameObject);
            }
            else if (other.CompareTag(armorTag))
            {
                HandleArmorCollision(other.gameObject);
            }
        }

        private void HandleHealthCollision(GameObject healthObject)
        {
            _health.AddHealth(10);
            healthObject.SetActive(false);
        }

        private void HandleDamageCollision(GameObject damageObject)
        {
            _health.TakeDamage(5);
            damageObject.SetActive(false);
        }

        private void HandleMoneyCollision(GameObject moneyObject)
        {
            UiController.instance.AddCoin(5);
            _playerUpgrade.AddMoneyUpgrade(5);
            ActiveScoreText(5);
            moneyObject.SetActive(false);
        }

        private void HandleArmorCollision(GameObject armorObject)
        {
            _health.ActiveShield();
            armorObject.SetActive(false);
        }

        private void ActiveScoreText(int value)
        {
            var scoreParticle = Instantiate(scoreText, spawnScoreTextPoint.position, spawnScoreTextPoint.rotation);
            var scoreT = scoreParticle.GetComponentInChildren<Text>();

            if (value >= 0)
            {
                scoreT.text = "+" + value;
                scoreT.color = Color.green;
            }
            else
            {
                scoreT.text = value.ToString();
                scoreT.color = Color.red;
            }

            var scorePosition = scoreParticle.position;
            scoreParticle.DOLocalMoveY(scorePosition.y + 2, 1);
            scoreT.DOFade(0, 1).OnComplete(() => Destroy(scoreParticle.gameObject));
        }
    }
}
