using Controllers;
using UnityEngine;
using Storage;

namespace Car.Player
{
    public class PlayerUpgradeController : MonoBehaviour
    {
        [Header("Upgrade Settings")]
        [SerializeField] private UpgradeObjectHolder[] upgradeObjectHolders;
        [SerializeField] private float addHealth;
        [SerializeField] private int addPriceHealth;
        [SerializeField] private int upgradeHealthPrice;
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
            InitializeControllers();
            InitializePrices();
        }

        private void Start()
        {
            UiController.instance.UpdateUpgradeLevel(0, 1);
            LoadPlayerUpgrades();
            SetupUiUpdates();
        }

        private void InitializeControllers()
        {
            _playerHealth = GetComponent<HealthController>();
            _playerShoot = GetComponent<CarShootController>();
        }

        private void InitializePrices()
        {
            _healthPrice = upgradeHealthPrice;
            _damagePrice = upgradeDamagePrice;
        }

        private void LoadPlayerUpgrades()
        {
            int healthLevel = PlayerPrefsController.GetUpgradeHealthNumber();
            int damageLevel = PlayerPrefsController.GetUpgradeDamageNumber();

            _playerHealth.UpgradeMaxHealth(addHealth * healthLevel);
            _healthPrice += addPriceHealth * healthLevel;

            _playerShoot.UpgradeBulletDamage(addDamage * damageLevel);
            _damagePrice += addPriceDamage * damageLevel;

            UiController.instance.UpdateHealthText(healthLevel + 1, _healthPrice);
            UiController.instance.UpdateDamageText(damageLevel + 1, _damagePrice);
        }

        private void SetupUiUpdates()
        {
            UiController.instance.upgradeHealthButton.onClick.AddListener(UpgradeHealth);
            UiController.instance.upgradeDamageButton.onClick.AddListener(UpgradeDamage);
        }

        public void UpgradeHealth()
        {
            if (TrySpendCurrency(ref _healthPrice))
            {
                _playerHealth.UpgradeMaxHealth(addHealth);
                IncrementUpgradeLevel(PlayerPrefsController.SetUpgradeHealthNumber);
            }
        }

        public void UpgradeDamage()
        {
            if (TrySpendCurrency(ref _damagePrice))
            {
                _playerShoot.UpgradeBulletDamage(addDamage);
                IncrementUpgradeLevel(PlayerPrefsController.SetUpgradeDamageNumber);
            }
        }

        private bool TrySpendCurrency(ref int price)
        {
            int money = PlayerPrefsController.GetTotalCurrency();
            if (money < price) return false;

            PlayerPrefsController.SetCurrency(money - price);
            price += price == _healthPrice ? addPriceHealth : addPriceDamage;
            return true;
        }

        private void IncrementUpgradeLevel(System.Action<int> upgradeSetter)
        {
            int upgradeLevel = PlayerPrefsController.GetUpgradeHealthNumber();
            upgradeSetter(upgradeLevel + 1);
            UiController.instance.UpdateHealthText(upgradeLevel + 1, _healthPrice);
            UiController.instance.UpdateCoinText();
        }

        public void AddMoneyUpgrade(int value)
        {
            _currentMoney += value;
            UpdateUpgradeLevel();
        }

        private void UpdateUpgradeLevel()
        {
            int levelUpgrade = CalculateLevelUpgrade();
            float bar = (float)_currentMoney / upgradeObjectHolders[levelUpgrade].targetMoney;
            UiController.instance.UpdateUpgradeLevel(bar, levelUpgrade + 1);
        }

        private int CalculateLevelUpgrade()
        {
            int levelUpgrade = 0;
            foreach (var upgradeHolder in upgradeObjectHolders)
            {
                if (_currentMoney >= upgradeHolder.targetMoney)
                {
                    levelUpgrade++;
                    ActivateUpgradeObjects(upgradeHolder);
                }
            }
            return Mathf.Clamp(levelUpgrade, 0, upgradeObjectHolders.Length - 1);
        }

        private void ActivateUpgradeObjects(UpgradeObjectHolder upgradeHolder)
        {
            if (!upgradeHolder.active && _currentMoney >= upgradeHolder.targetMoney)
            {
                foreach (var obj in upgradeHolder.objects)
                {
                    obj.SetActive(true);
                }
                upgradeHolder.active = true;
            }
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
