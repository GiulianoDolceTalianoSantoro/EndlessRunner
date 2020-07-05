using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public TrackManager trackManager;
    public Character character;
    public CharacterCollider characterCollider;
    public float laneChangeSpeed = 1.0f;

    protected bool _isInvincible;
    protected bool _isRunning;

    protected float m_JumpStart;

    protected bool _isJumping;

    public bool IsJumping
    {
        get { return _isJumping; }
    }

    public float jumpLength = 2.0f;     // Distance jumped
    public float jumpHeight = 1.2f;

    protected Vector2 _startingTouch;

    protected bool _isSwiping;

    protected int _currentLane = _startingLane;
    protected Vector3 _targetPosition = Vector3.zero;

    protected readonly Vector3 _startingPosition = Vector3.forward * 2f;

    protected const int _startingLane = 1;
    protected const float _groundingSpeed = 80f;
    protected const float k_TrackSpeedToJumpAnimSpeedRatio = 0.6f;

    public static float distanceTravelled = 0f;
    Vector3 lastPosition;

    public void CheatInvincible(bool invincible)
    {
        _isInvincible = invincible;
    }

    public bool IsCheatInvincible()
    {
        return _isInvincible;
    }

    public void Init()
    {
        transform.position = _startingPosition;
        _targetPosition = Vector3.zero;

        _currentLane = _startingLane;
        characterCollider.transform.localPosition = Vector3.zero;

        //currentLife = maxLife;

        //m_ObstacleLayer = 1 << LayerMask.NameToLayer("Obstacle");
    }

    // Called at the beginning of a run or rerun
    public void Begin()
    {
        _isRunning = false;
        //character.animator.SetBool(s_DeadHash, false);

        characterCollider.Init();
    }

    public void StartRunning()
    {
        StartMoving();
        //if (character.animator)
        //{
        //    character.animator.Play(s_RunStartHash);
        //    character.animator.SetBool(s_MovingHash, true);
        //}
    }

    public void StartMoving()
    {
        _isRunning = true;
    }

    public void StopMoving()
    {
        _isRunning = false;
        trackManager.StopMove();
        //if (character.animator)
        //{
        //    character.animator.SetBool(s_MovingHash, false);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        InputManager();
        CalculateDistanceTravelled();
    }

    void InputManager()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // Use key input in editor or standalone
        // disabled if it's tutorial and not thecurrent right tutorial level (see func TutorialMoveCheck)

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
#else
        // use touch input on mobile
        if (Input.touchCount == 1)
        {
			if(_isSwiping)
			{
                Vector2 diff = Input.GetTouch(0).position - _startingTouch;

                // Put difference in Screen ratio, but using only width, so the ratio is the same on both
                // axes (otherwise we would have to swipe more vertically...)
                diff = new Vector2(diff.x/Screen.width, diff.y/Screen.width);

				if(diff.magnitude > 0.01f) //we set the swip distance to trigger movement to 1% of the screen width
				{
					if(Mathf.Abs(diff.y) > Mathf.Abs(diff.x))
					{
						if( diff.y > 0)
						{
							Jump();
						}
					}
					else
					{
						if(diff.x < 0)
						{
							ChangeLane(-1);
						}
						else
						{
							ChangeLane(1);
						}
					}
						
					_isSwiping = false;
				}
            }

        	// Input check is AFTER the swip test, that way if TouchPhase.Ended happen a single frame after the Began Phase
			// a swipe can still be registered (otherwise, m_IsSwiping will be set to false and the test wouldn't happen for that began-Ended pair)
			if(Input.GetTouch(0).phase == TouchPhase.Began)
			{
				_startingTouch = Input.GetTouch(0).position;
				_isSwiping = true;
			}
			else if(Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				_isSwiping = false;
			}
        }
#endif

        Vector3 verticalTargetPosition = _targetPosition;

        if (_isJumping)
        {
            if (trackManager.isMoving)
            {
                // Same as with the sliding, we want a fixed jump LENGTH not fixed jump TIME. Also, just as with sliding,
                // we slightly modify length with speed to make it more playable.
                float correctJumpLength = jumpLength * (1.0f + trackManager.speedRatio);
                float ratio = (trackManager.worldDistance - m_JumpStart) / correctJumpLength;
                if (ratio >= 1.0f)
                {
                    _isJumping = false;
                    //character.animator.SetBool(s_JumpingHash, false);
                }
                else
                {
                    verticalTargetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeight;
                }
            }
            else if (!AudioListener.pause) //use AudioListener.pause as it is an easily accessible singleton & it is set when the app is in pause too
            {
                verticalTargetPosition.y = Mathf.MoveTowards(verticalTargetPosition.y, 0, _groundingSpeed * Time.deltaTime);
                if (Mathf.Approximately(verticalTargetPosition.y, 0f))
                {
                    //character.animator.SetBool(s_JumpingHash, false);
                    _isJumping = false;
                }
            }
        }

        characterCollider.transform.localPosition = Vector3.MoveTowards(characterCollider.transform.localPosition, verticalTargetPosition, laneChangeSpeed * Time.deltaTime);
    }

    void Jump()
    {
        if (!_isRunning)
		    return;
	    
        if (!_isJumping)
        {
			float correctJumpLength = jumpLength * (1.0f + trackManager.speedRatio);
			m_JumpStart = trackManager.worldDistance;
            float animSpeed = k_TrackSpeedToJumpAnimSpeedRatio * (trackManager.speed / correctJumpLength);

            //character.animator.SetFloat(s_JumpingSpeedHash, animSpeed);
            //character.animator.SetBool(s_JumpingHash, true);
			_isJumping = true;
        }
    }

    public void StopJumping()
    {
        if (_isJumping)
        {
            //character.animator.SetBool(s_JumpingHash, false);
            _isJumping = false;
        }
    }

    public void ChangeLane(int direction)
    {
        if (!_isRunning)
            return;

        int targetLane = _currentLane + direction;

        if (targetLane < 0 || targetLane > 2)
            return;

        _currentLane = targetLane;
        _targetPosition = new Vector3((_currentLane - 1) * trackManager.laneOffset, 0, 0);
    }

    void CalculateDistanceTravelled()
    {
        distanceTravelled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        var roundedDistance = Mathf.RoundToInt(distanceTravelled);
    }
}

enum PlayerPosition
{
    Left,
    Middle,
    Right
}