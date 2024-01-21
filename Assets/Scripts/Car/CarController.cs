using System.Collections;
using Dreamteck.Splines;
using UnityEngine;
using Levels;
using Controllers;
using Other;

namespace Car
{
    public enum StartMoveType
    {
        LoadScene,
        LoadLevel,
        StartGame
    }

    [RequireComponent(typeof(SplineFollower))]
    public class CarController : MonoBehaviour
    {
        public float speedForward;
        [SerializeField] private StartMoveType startMoveType;
        [SerializeField] private RotateObject[] wheels;

        [SerializeField] protected float speedHorizontal;
        [SerializeField] protected float maxHorizontalMove;
        [SerializeField] protected Joystick joystickCar;

        public SplineFollower Follower { get; private set; }

        protected bool canMove;
        private float _xMove;
        private float _turnAngle;
        private Animator _anim;
        private static readonly int HorizontalTurning = Animator.StringToHash("HorizontalTurning");

        #region Unity Functions

        protected virtual void Awake()
        {
            AttachLevelManagerEvents();
            InitializeComponents();
        }

        protected virtual void Start()
        {
            InitializeMovement();
        }

        protected virtual void Update()
        {
            if (canMove && joystickCar)
            {
                PerformHorizontalMove();
            }
        }

        protected virtual void OnDestroy()
        {
            DetachLevelManagerEvents();
        }

        #endregion

        #region Initialization Functions

        private void InitializeComponents()
        {
            Follower = GetComponent<SplineFollower>();
            _anim = GetComponent<Animator>();
            Follower.followSpeed = 0;

            if (!Follower.spline)
            {
                StartCoroutine(FindSplineComputer());
            }
        }

        private void InitializeMovement()
        {
            if (startMoveType == StartMoveType.LoadScene)
            {
                ActivateCar();
            }
        }

        #endregion

        #region Movement Functions

        protected virtual void PerformHorizontalMove()
        {
            float horizontalInput = joystickCar.Horizontal;
            float offsetY = Follower.motion.offset.y;
            _turnAngle = Mathf.Lerp(_turnAngle, horizontalInput, 5 * Time.deltaTime);
            _anim.SetFloat(HorizontalTurning, _turnAngle);
            _xMove += horizontalInput * speedHorizontal * Time.deltaTime;
            _xMove = Mathf.Clamp(_xMove, -maxHorizontalMove, maxHorizontalMove);

            Follower.motion.offset = new Vector2(_xMove, offsetY);
        }

        protected virtual void ActivateCar()
        {
            canMove = true;
            Follower.followSpeed = speedForward;
            ActivateWheels();
        }

        protected virtual void DeactivateCar()
        {
            canMove = false;
            Follower.followSpeed = 0;
            DeactivateWheels();
        }

        private void ActivateWheels()
        {
            foreach (var wheel in wheels)
            {
                wheel.Active();
            }
        }

        private void DeactivateWheels()
        {
            foreach (var wheel in wheels)
            {
                wheel.Diactive();
            }
        }

        #endregion

        #region Level Manager Event Handlers

        private void AttachLevelManagerEvents()
        {
            LevelManager.OnLevelStart += OnLevelStart;
            LevelManager.OnLevelComplete += OnLevelComplete;
            LevelManager.OnLevelLoad += OnLoadLevel;
            LevelManager.OnLevelFail += OnLevelFail;
        }

        private void DetachLevelManagerEvents()
        {
            LevelManager.OnLevelStart -= OnLevelStart;
            LevelManager.OnLevelComplete -= OnLevelComplete;
            LevelManager.OnLevelLoad -= OnLoadLevel;
            LevelManager.OnLevelFail -= OnLevelFail;
        }

        protected virtual void OnLevelStart(Level level)
        {
            if (startMoveType == StartMoveType.StartGame)
            {
                ActivateCar();
            }
        }

        protected virtual void OnLevelComplete(Level level)
        {
            Invoke(nameof(DeactivateCar), 3);
        }

        protected virtual void OnLoadLevel(Level level)
        {
            if (startMoveType == StartMoveType.LoadLevel)
            {
                ActivateCar();
            }
        }

        protected virtual void OnLevelFail(Level level)
        {
            DeactivateCar();
        }

        #endregion

        #region Coroutine

        private IEnumerator FindSplineComputer()
        {
            while (!Follower.spline)
            {
                yield return new WaitForEndOfFrame();
                var computer = FindObjectOfType<SplineComputer>();
                if (computer)
                {
                    Follower.spline = computer;
                }
            }
        }
        #endregion
    }
}