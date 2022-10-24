using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

namespace Car.Enemy
{
    public class EnemyHolder : MonoBehaviour
    {
        public static EnemyHolder Instance;
        
        private int _numberEnemy;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
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
