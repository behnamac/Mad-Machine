using Levels;
using Storage;
using UnityEngine;

namespace Controllers
{
    public class LevelManager : MonoBehaviour
    {
        #region DELEGATE

        public delegate void LevelLoadHandler(Level levelData);

        public delegate void LevelStartHandler(Level levelData);

        public delegate void LevelStageCompleteHandler(Level levelData, int stageIndex = 0);

        public delegate void LevelCompleteHandler(Level levelData);

        public delegate void LevelFailHandler(Level levelData);

        #endregion

        #region EVENTS

        /// <summary>
        ///     Level yüklendikten sonra çalışan ve level'ın yüklendiğini belirten event.
        /// </summary>
        public static LevelLoadHandler OnLevelLoad;

        /// <summary>
        /// Level oynamaya başladığında çalışan event.
        /// </summary>
        public static LevelStartHandler OnLevelStart;

        /// <summary>
        /// Level stage'lerden oluşuyor ise her stage tamamlandığında çalışan event
        /// </summary>
        public static LevelStageCompleteHandler OnLevelStageComplete;

        /// <summary>
        /// Level tamamlandığında çalışan event
        /// </summary>
        public static LevelCompleteHandler OnLevelComplete;

        /// <summary>
        /// Level başarısız olduğunda çalışan event
        /// </summary>
        public static LevelFailHandler OnLevelFail;

        #endregion

        #region PUBLIC FIELDS / PROPS

        public static LevelManager Instance { get; private set; }

        #endregion

        #region SERIALIZE PRIVATE FIELDS

        // Level'ların bulunduğu kaynak
        [SerializeField] private LevelSource levelSource;

        // Level'ın spawn edileceği container
        [SerializeField] private GameObject levelSpawnPoint;

        // Maksimum level sonrası başa döndüğünde kaçıncı level'dan başlayacağını belirler. Default değeri 1 dir. 
        [SerializeField] private int loopLevelsStartIndex = 1;

        // Maksimum level sonrası yüklenecek levelların random olarak gelip gelmeyeceğini belirler, Default değeri True'dur. 
        [SerializeField] private bool loopLevelGetRandom = true;

        #endregion

        #region PRIVATE FIELDS

        private GameObject _activeLevel;

        #endregion

        #region PRIVATE METHODS

        private void CheckRepeatLevelIndex()
        {
            if (loopLevelsStartIndex < levelSource.levelData.Length) return;
            loopLevelsStartIndex = 0;
        }

        private GameObject GetLevel()
        {
            if (PlayerPrefsController.GetLevelIndex() >= levelSource.levelData.Length)
            {
                if (loopLevelGetRandom)
                {
                    var levelIndex = Random.Range(loopLevelsStartIndex, levelSource.levelData.Length - 1);
                    PlayerPrefsController.SetLevelIndex(levelIndex);
                }
            }

            var level = levelSource.levelData[PlayerPrefsController.GetLevelIndex()];

            var levelData = level.GetComponent<Level>();

            levelData.levelIndex = PlayerPrefsController.GetLevelIndex();
            levelData.levelNumber = PlayerPrefsController.GetLevelNumber();

            return level;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        ///     Sıradaki level'ı yükleyen metod
        /// </summary>
        public void LevelLoad()
        {
            _activeLevel = Instantiate(GetLevel(), levelSpawnPoint.transform, false);
            OnLevelLoad?.Invoke(_activeLevel.GetComponent<Level>());
        }

        /// <summary>
        ///     Son yüklenen level'ı başlatan method
        /// </summary>
        public void LevelStart()
        {
            OnLevelStart?.Invoke(_activeLevel.GetComponent<Level>());
        }

        /// <summary>
        /// Yüklenen level içerisinde stage'ler var ise her stage tamamlandığında çağrılacak methods
        /// </summary>
        public void LevelStageComplete(int stageIndex = 0)
        {
            OnLevelStageComplete?.Invoke(_activeLevel.GetComponent<Level>(), stageIndex);
        }

        /// <summary>
        ///     Oynanan level tamamlandığında çağrılacak olan methods
        /// </summary>
        public void LevelComplete()
        {
            // Sonraki level index değeri atanıyor
            PlayerPrefsController.SetLevelIndex(PlayerPrefsController.GetLevelIndex() + 1);

            // Sonraki level numarası atanıyor
            PlayerPrefsController.SetLevelNumber(PlayerPrefsController.GetLevelNumber() + 1);
            
            OnLevelComplete?.Invoke(_activeLevel.GetComponent<Level>());
        }

        /// <summary>
        ///     Oynanan level başarısız olduğunda çağrılacak olan method
        /// </summary>
        public void LevelFail()
        {
            OnLevelFail?.Invoke(_activeLevel.GetComponent<Level>());
        }

        #endregion

        #region UNITY EVENT METHODS

        private void Awake()
        {
            CheckRepeatLevelIndex();
            Instance = this;
        }

        private void Start() => LevelLoad();

        #endregion
    }
}