using UnityEngine;

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

    public float jumpLength = 2.0f;
    public float jumpHeight = 1.2f;

    protected Vector2 _startingTouch;

    protected bool _isSwiping;

    protected int _currentLane = _startingLane;
    protected Vector3 _targetPosition = Vector3.zero;

    protected readonly Vector3 _startingPosition = Vector3.forward * 2f;

    protected const int _startingLane = 1;
    protected const float _groundingSpeed = 80f;
    protected const float k_TrackSpeedToJumpAnimSpeedRatio = 0.6f;

    protected int m_CurrentLife;
    public int maxLife = 3;
    public int currentLife { get { return m_CurrentLife; } set { m_CurrentLife = value; } }

    protected int m_Points;
    public int points { get { return m_Points; } set { m_Points = value; } }

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
        //TestDebug.Debugging("player init");

        transform.position = _startingPosition;
        _targetPosition = Vector3.zero;

        _currentLane = _startingLane;
        characterCollider.transform.localPosition = Vector3.zero;

        currentLife = maxLife;
        points = 0;
    }

    // Called at the beginning of a run or rerun
    public void Begin()
    {
        _isRunning = false;

        //TestDebug.Debugging("player begin");

        characterCollider.Init();
    }

    public void StartRunning()
    {
        StartMoving();
    }

    public void StartMoving()
    {
        //TestDebug.Debugging("start");

        _isRunning = true;
    }

    public void StopMoving()
    {
        _isRunning = false;
        trackManager.StopMove();
    }

    // Update is called once per frame
    void Update()
    {
        InputManager();
    }

    void InputManager()
    {
        // Use key input in editor or standalone

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
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            StopJumping();
        }

        // use touch input on mobile
        if (Input.touchCount == 1)
        {
			if(_isSwiping)
			{
                Vector2 diff = Input.GetTouch(0).position - _startingTouch;

                diff = new Vector2(diff.x/Screen.width, diff.y/Screen.width);

				if(diff.magnitude > 0.01f) //we set the swip distance to trigger movement to 1% of the screen width
				{
					if(Mathf.Abs(diff.y) > Mathf.Abs(diff.x))
					{
                        if(diff.y < 0)
                        {
                            StopJumping();
                        }
						else
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

        Vector3 verticalTargetPosition = _targetPosition;

        if (_isJumping)
        {
            if (trackManager.isMoving)
            {
                float correctJumpLength = jumpLength * (1.0f + trackManager.speedRatio);
                float ratio = (trackManager.worldDistance - m_JumpStart) / correctJumpLength;
                if (ratio >= 1.0f)
                {
                    _isJumping = false;
                }
                else
                {
                    verticalTargetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeight;
                }
            }
            else if (!AudioListener.pause)
            {
                verticalTargetPosition.y = Mathf.MoveTowards(verticalTargetPosition.y, 0, _groundingSpeed * Time.deltaTime);
                if (Mathf.Approximately(verticalTargetPosition.y, 0f))
                {
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
			m_JumpStart = trackManager.worldDistance;

			_isJumping = true;
        }
    }

    public void StopJumping()
    {
        if (_isJumping)
        {
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
}