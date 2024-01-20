using Controllers;
using UnityEngine;

namespace Car.Enemy
{
    public class EnemyHolder : MonoBehaviour
    {
        // Singleton Instance
        [SerializeField] private static EnemyHolder instance;

        public static EnemyHolder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<EnemyHolder>();
                    if (instance == null)
                    {
                        Debug.LogError("EnemyHolder instance not found in the scene.");
                    }
                }
                return instance;
            }
        }

        private int _numberEnemy;

        private void Start()
        {
            // Initialize _numberEnemy at the start of the level
            _numberEnemy = FindObjectsOfType<EnemyMoveController>().Length;
        }

        public void RemoveEnemy()
        {
            _numberEnemy--;

            if (_numberEnemy <= 0)
            {
                LevelManager.Instance.LevelComplete();
            }
        }
    }
}
