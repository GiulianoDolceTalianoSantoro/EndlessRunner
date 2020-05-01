using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    GameManager gameManager;

    Rigidbody rb;

    /// <summary>
    /// Speed values
    /// </summary>
    float currentSpeed = 20f;
    float minSpeed;
    float maxSpeed = 60f;
    public float accelerationTime = 80f;
    float time;
    [Range(5f, 10f)]
    public float sideSpeed = 5f;

    /// <summary>
    /// Jump values
    /// </summary>
    public LayerMask floorLayer;
    SphereCollider coll;
    public float jumpForce;
    public float fallMultiplier;

    float[] positions;

    [SerializeField]
    PlayerPosition currentPosition;

    private Touch touch;
    private Vector3 beginTouchPosition;
    private Vector3 endTouchPosition;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        rb = GetComponent<Rigidbody>();
        coll = GetComponent<SphereCollider>();

        time = 0;
        minSpeed = currentSpeed;

        float leftPositionX = -5f;
        float middlePositionX = 0f;
        float rightPositionX = 5f;

        positions = new float[] { leftPositionX, middlePositionX, rightPositionX };

        SetInitialPosition();
    }

    // Update is called once per frame
    void Update()
    {
        IncreasePlayerSpeed();
        InputManager();

        InputManagerForTesting();
    }

    void SetInitialPosition()
    {
        currentPosition = PlayerPosition.Middle;

        transform.position = new Vector3(positions[(int)currentPosition], transform.position.y, transform.position.z);
    }

    void InputManager()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    beginTouchPosition = touch.position;
                    break;
                case TouchPhase.Ended:
                    endTouchPosition = touch.position;

                    if (beginTouchPosition == endTouchPosition)
                    {
                        return;
                    }

                    if (beginTouchPosition.x - 100f > endTouchPosition.x)
                    {
                        MoveToExactPosition(ScreenPosition.Left);
                    }
                    else if (beginTouchPosition.x + 100f < endTouchPosition.x)
                    {
                        MoveToExactPosition(ScreenPosition.Right);
                    }
                    else if (beginTouchPosition.y < endTouchPosition.y)
                    {
                        Jump();
                    }

                    break;
            }
        }

        GravityBalance();
    }

    void InputManagerForTesting()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveToExactPosition(ScreenPosition.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveToExactPosition(ScreenPosition.Right);
        }
    }

    void Jump()
    {
        Vector3 vector = new Vector3(coll.bounds.center.x, coll.bounds.min.y, coll.bounds.center.z);
        var isOnFloor = Physics.CheckCapsule(coll.bounds.center, vector, coll.radius, floorLayer);

        if (isOnFloor)
        {
            rb.velocity += Vector3.up * jumpForce;
        }
    }

    void GravityBalance()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    void MoveToExactPosition(ScreenPosition screenPosition)
    {
        if (screenPosition == ScreenPosition.Left)
        {
            switch (currentPosition)
            {
                case PlayerPosition.Left:
                    currentPosition = PlayerPosition.Left;
                    break;
                case PlayerPosition.Middle:
                    currentPosition = PlayerPosition.Left;
                    break;
                case PlayerPosition.Right:
                    currentPosition = PlayerPosition.Middle;
                    break;
            }
        }
        else if (screenPosition == ScreenPosition.Right)
        {
            switch (currentPosition)
            {
                case PlayerPosition.Left:
                    currentPosition = PlayerPosition.Middle;
                    break;
                case PlayerPosition.Middle:
                    currentPosition = PlayerPosition.Right;
                    break;
                case PlayerPosition.Right:
                    currentPosition = PlayerPosition.Right;
                    break;
            }
        }

        transform.DOMoveX(positions[(int)currentPosition], sideSpeed * Time.deltaTime);
        // We should add force instead of using DOTween until the position we need is reached
    }

    void IncreasePlayerSpeed()
    {
        currentSpeed = Mathf.SmoothStep(minSpeed, maxSpeed, time / accelerationTime);
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
        time += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Plataform"))
        {
            rb.velocity = Vector3.zero;
            currentSpeed = 0;
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.normal.y > 0)
                {
                    return;
                }
                else
                {
                    gameManager.RestartLevel();
                }
            }
        }
    }
}

enum ScreenPosition
{
    Left,
    Right
}

enum PlayerPosition
{
    Left,
    Middle,
    Right
}