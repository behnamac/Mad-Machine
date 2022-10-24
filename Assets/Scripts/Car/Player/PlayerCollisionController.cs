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
        private void Awake()
        {
            _health = GetComponent<HealthController>();
            _playerUpgrade = GetComponent<PlayerUpgradeController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Health"))
            {
                _health.AddHealth(10);
                other.gameObject.SetActive(false);
            }
            else if (other.gameObject.CompareTag("Damage"))
            {
                _health.TakeDamage(5);
                other.gameObject.SetActive(false);
            }
            else if (other.gameObject.CompareTag("Money"))
            {
                UiController.instance.AddCoin(5);
                _playerUpgrade.AddMoneyUpgrade(5);
                ActiveScoreSText(5, spawnScoreTextPoint);
                other.gameObject.SetActive(false);
            }
            else if (other.gameObject.CompareTag("Armor"))
            {
                _health.ActiveShield();
                other.gameObject.SetActive(false);
            }
        }

        private void ActiveScoreSText(int value, Transform spawnPoint)
        {
            var scoreParticle = Instantiate(scoreText, spawnPoint.position, spawnPoint.rotation); 
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
            scoreT.DOFade(0, 1);
            Destroy(scoreParticle.gameObject, 1);
        }
    }
}
