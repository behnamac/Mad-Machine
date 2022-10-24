using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Other
{
    public class RotateObject : MonoBehaviour
    {
        [SerializeField] private Vector3 speedAndAxisRotate;
        [SerializeField] private bool activeOnAwake;

        private bool _canRotate;

        private void Awake()
        {
            _canRotate = activeOnAwake;
        }

        private void Update()
        {
            if (_canRotate)
            {
                Rotate();
            }
        }

        private void Rotate()
        {
            transform.Rotate(speedAndAxisRotate * Time.deltaTime);
        }

        public void Active() => _canRotate = true;
        public void Diactive() => _canRotate = false;
    }
}
