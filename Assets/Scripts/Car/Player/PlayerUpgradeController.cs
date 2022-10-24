using Controllers;
using UnityEngine;
using Storage;

namespace Car.Player
{
    public class PlayerUpgradeController : MonoBehaviour
    {
        [SerializeField] private UpgradeObjectHolder[] upgradeObjectHolders;
        
        [Header("Upgrade Health")]
        [SerializeField] private float addHealth;
        [SerializeField] private int addPriceHealth;
        [SerializeField] private int upgradeHealthPrice;

        [Header("Upgrade Damage")]
        [SerializeField] private float addDamage;
        [SerializeField] private int addPriceDamage;
        [SerializeField] private int upgradeDamagePrice;
        
        private int _currentMoney;
        private int _healthPrice;
        private int _damagePrice;
        private HealthController _playerHealth;
        private CarShootController _playerShoot;

        private void Awake()
        {
            _playerHealth = GetComponent<HealthController>();
            _playerShoot = GetComponent<CarShootController>();
            _healthPrice = upgradeHealthPrice;
            _damagePrice = upgradeDamagePrice;

            UiController.instance.UpdateUpgradeLevel(0, 1);
        }

        private void Start()
        {
            int healthNum = PlayerPrefsController.GetUpgradeHealthNumber();
            int damageNum = PlayerPrefsController.GetUpgradeDamageNumber();
            _playerHealth.UpgradeMaxHealth(addHealth * healthNum);
            _healthPrice += addPriceHealth * healthNum;
            _playerShoot.UpgradeBulletDamage(addDamage * damageNum);
            _damagePrice += addPriceDamage * damageNum;
            
            UiController.instance.UpdateHealthText(healthNum + 1, _healthPrice);
            UiController.instance.UpdateDamageText(damageNum + 1, _damagePrice);
            UiController.instance.upgradeHealthButton.onClick.AddListener(UpgradeHealth);
            UiController.instance.upgradeDamageButton.onClick.AddListener(UpgradeDamage);
        }

        public void UpgradeHealth()
        {
            int money = PlayerPrefsController.GetTotalCurrency();
            if (money < _healthPrice) return;

            PlayerPrefsController.SetCurrency(money - _healthPrice);
            _playerHealth.UpgradeMaxHealth(addHealth);
            _healthPrice += addPriceHealth;
            int healthNum = PlayerPrefsController.GetUpgradeHealthNumber();
            PlayerPrefsController.SetUpgradeHealthNumber(healthNum + 1);

            UiController.instance.UpdateHealthText(PlayerPrefsController.GetUpgradeHealthNumber() + 1, _healthPrice);
            UiController.instance.UpdateCoinText();
        }
        public void UpgradeDamage()
        {
            int money = PlayerPrefsController.GetTotalCurrency();
            if (money < _damagePrice) return;

            PlayerPrefsController.SetCurrency(money - _damagePrice);
            _playerShoot.UpgradeBulletDamage(addDamage);
            _damagePrice += addPriceDamage;
            int damageNum = PlayerPrefsController.GetUpgradeDamageNumber();
            PlayerPrefsController.SetUpgradeDamageNumber(damageNum + 1);

            UiController.instance.UpdateDamageText(PlayerPrefsController.GetUpgradeDamageNumber() + 1, _damagePrice);
            UiController.instance.UpdateCoinText();
        }

        public void AddMoneyUpgrade(int value)
        {
            _currentMoney += value;
            int levelUpgrade = 0;
            for (int i = 0; i < upgradeObjectHolders.Length; i++)
            {
                var upgradeHolder = upgradeObjectHolders[i];
                if (_currentMoney >= upgradeHolder.targetMoney)
                    levelUpgrade++;

                if (!upgradeHolder.active && _currentMoney >= upgradeHolder.targetMoney)
                {
                    for (int j = 0; j < upgradeHolder.objects.Length; j++)
                    {
                        upgradeHolder.objects[j].SetActive(true);
                    }
                }
            }

            levelUpgrade = Mathf.Clamp(levelUpgrade, 0, upgradeObjectHolders.Length - 1);
            // ReSharper disable once PossibleLossOfFraction
            float bar = (float)_currentMoney / upgradeObjectHolders[levelUpgrade].targetMoney;
            UiController.instance.UpdateUpgradeLevel(bar, levelUpgrade + 1);
        }

        [System.Serializable]
        private class UpgradeObjectHolder
        {
            public int targetMoney;
            public GameObject[] objects;
            [HideInInspector] public bool active;
        }
    }
}
