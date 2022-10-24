using System;
using Levels;
using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController instance { get; private set; }


        #region SERIALIZE FIELDS

        [SerializeField] private Transform target;
        [SerializeField] private float followSpeed = 0.1f;

        private bool _canFollow;
        #endregion

        #region PRIVATE METHODS

        private void Initialize()
        {
        }

        private void SmoothFollow()
        {
            var thisTransform = transform;
            var thisPosition = thisTransform.position;
            var thisRotation = thisTransform.rotation;
            var targetPosition = target.position;
            var targetRotation = target.rotation;

            thisTransform.position = Vector3.Lerp(thisPosition, targetPosition, followSpeed * Time.deltaTime);
            thisTransform.rotation = Quaternion.Lerp(thisRotation, targetRotation, 3 * Time.deltaTime);
        }

        #endregion

        #region UNITY EVENT METHODS

        private void Awake()
        {
            if (instance == null) instance = this;

            _canFollow = true;

            LevelManager.OnLevelFail += OnLevelFail;
        }

        private void Start() => Initialize();

        private void LateUpdate()
        {
            if (_canFollow)
            {
                SmoothFollow();
            }
        }

        private void OnDestroy()
        {
            LevelManager.OnLevelFail -= OnLevelFail;
        }

        private void OnLevelFail(Level level)
        {
            _canFollow = false;
        }

        #endregion
    }
}