using System.Collections;
using Dreamteck.Splines;
using UnityEngine;
using Levels;
using Controllers;
using Other;
using UnityEngine.Serialization;

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
        public StartMoveType startMoveType;
        public float speedForward;
        public RotateObject[] wheels;
        
        protected float SpeedHorizontal;
        protected float MaxHorizontalMove;
        protected Joystick JoystickCar;
        
        public SplineFollower follower { get; set; }

        protected bool CanMove;
        private float _xMove;
        private float _turnAngle;
        private Animator _anim;
        private static readonly int HorizontalTurning = Animator.StringToHash("HorizontalTurning");

        #region Unity Functions
        
        protected virtual void Awake()
        {
            LevelManager.OnLevelStart += OnLevelStart;
            LevelManager.OnLevelComplete += OnLevelComplete;
            LevelManager.OnLevelLoad += OnLoadLevel;
            LevelManager.OnLevelFail += OnLevelFail;
            
            follower = GetComponent<SplineFollower>();
            _anim = GetComponent<Animator>();
            follower.followSpeed = 0;

            if (!follower.spline)
            {
                StartCoroutine(FindSplineComputer());
            }
        }

        protected virtual void Start()
        {
            if (startMoveType == StartMoveType.LoadScene)
            {
                CanMove = true;
                follower.followSpeed = speedForward;
            }

        }

        protected virtual void Update()
        {
            if (CanMove && JoystickCar)
            {
                HorizontalMove();
            }
        }

        protected virtual void OnDestroy()
        {
            LevelManager.OnLevelStart -= OnLevelStart;
            LevelManager.OnLevelComplete -= OnLevelComplete;
            LevelManager.OnLevelLoad -= OnLoadLevel;
            LevelManager.OnLevelFail -= OnLevelFail;
        }

        #endregion

        #region Custom Functions

        protected virtual void HorizontalMove()
        {
            var h = JoystickCar.Horizontal;
            var offsetY = follower.motion.offset.y;
            _turnAngle = Mathf.Lerp(_turnAngle, h, 5 * Time.deltaTime);
            _anim.SetFloat(HorizontalTurning, _turnAngle);
            _xMove += h * SpeedHorizontal * Time.deltaTime;
            _xMove = Mathf.Clamp(_xMove, -MaxHorizontalMove, MaxHorizontalMove);

            follower.motion.offset = new Vector2(_xMove, offsetY);
        }

        protected virtual void OnActiveCar()
        {
            CanMove = true;
            follower.followSpeed = speedForward;

            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].Active();
            }
        }
        protected virtual void OnDiactiveCar()
        {
            CanMove = false;
            follower.followSpeed = 0;
            
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].Diactive();
            }
        }

        #endregion

        #region EVENTs

        protected virtual void OnLevelStart(Level level)
        {
            if (startMoveType == StartMoveType.StartGame)
            {
                OnActiveCar();
            }
        }
        protected virtual void OnLevelComplete(Level level)
        {
            Invoke(nameof(OnDiactiveCar), 3);
        }
        protected virtual void OnLoadLevel(Level level)
        {
            if (startMoveType == StartMoveType.LoadLevel)
            {
                OnActiveCar();
            }
        }
        protected virtual void OnLevelFail(Level level)
        {
            OnDiactiveCar();
        }

        #endregion

        #region IEnumerators

        private IEnumerator FindSplineComputer()
        {
            while (!follower.spline)
            {
                yield return new WaitForEndOfFrame();
                var computer = FindObjectOfType<SplineComputer>();
                if (computer)
                {
                    follower.spline = computer;
                }
            }
        }

        #endregion
    }
}
