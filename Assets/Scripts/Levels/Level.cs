using UnityEngine;

namespace Levels
{
    public class Level : MonoBehaviour
    {
        #region PUBLIC FIELDS

        public int levelIndex;
        public int levelNumber;

        #endregion

        #region SERIALIZE FIELDS

        [SerializeField] private Material skyBox;
        [SerializeField] private Color fogColor;

        #endregion

        #region UNITY EVENT METHODS

        private void Awake()
        {
            RenderSettings.skybox = skyBox;
            RenderSettings.fogColor = fogColor;
        }

        #endregion
    }
}