using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    #region Private Fields
    GameManager gameManager;

    Rigidbody rb;

    float[] positions;
    PlayerPosition currentPosition;

    float currentSpeed = 20f;
    float minSpeed;

    float currentSideSpeed = 1f;
    float minSideSpeed;

    float time;

    SphereCollider coll;
    #endregion

    #region Public Fields
    [Header("Speed values")]
    [Tooltip("Player's maximum speed")]
    public float maxSpeed = 60f;
    [Tooltip("Player's maximum side movement speed")]
    public float maxSideSpeed = 15f;
    [Tooltip("Player's acceleration per time")]
    public float accelerationTime = 80f;
    

    [Header("Jump values")]
    [Tooltip("The layer of GameObjects where the Player will be able to jump over")]
    public LayerMask floorLayer;
    [Tooltip("Player's jump force")]
    public float jumpForce;
    [Tooltip("Player's amount to multiply for when falling")]
    public float fallMultiplier;
    #endregion

    public static float distanceTravelled = 0f;
    Vector3 lastPosition;

    /// <summary>
    /// Gesture Actions & values
    /// </summary>
    Action leftSwipe;
    Action rightSwipe;
    Action upSwipe;
    Action downSwipe;

    Action leftRotate;
    Action rightRotate;
    float rotateAmount = 5f;

    Action zoomIn;
    Action zoomOut;
    float zoomAmount = 1f;
    float zoomSpeed = 2f;

    float height;

    // Start is called before the first frame update
    void Start()
    {
        height = transform.localScale.y;

        gameManager = FindObjectOfType<GameManager>();

        rb = GetComponent<Rigidbody>();
        coll = GetComponent<SphereCollider>();

        time = 0;
        minSpeed = currentSpeed;
        minSideSpeed = currentSideSpeed;

        float leftPositionX = -5f;
        float middlePositionX = 0f;
        float rightPositionX = 5f;

        positions = new float[] { leftPositionX, middlePositionX, rightPositionX };

        SetInitialPosition();

        lastPosition = transform.position;

        leftSwipe = () => MoveToExactPosition(ScreenPosition.Left);
        rightSwipe = () => MoveToExactPosition(ScreenPosition.Right);
        upSwipe = () => Jump();
        downSwipe = () => Crouch();

        leftRotate = () => Rotate(-rotateAmount);
        rightRotate = () => Rotate(rotateAmount);

        zoomIn = () => Zoom(zoomAmount);
        zoomOut = () => Zoom(-zoomAmount);
    }

    // Update is called once per frame
    void Update()
    {
        IncreasePlayerSpeed();
        InputManager();
        CalculateDistanceTravelled();

        InputManagerForTesting();
    }

    void Rotate(float rotateAmt)
    {
        Camera.main.transform.Rotate(0f, 0f, rotateAmt);
    }

    void Zoom(float zoomAmt)
    {
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + (zoomAmt * zoomSpeed), 30, 90);
    }

    void SetInitialPosition()
    {
        currentPosition = PlayerPosition.Middle;

        transform.position = new Vector3(positions[(int)currentPosition], transform.position.y, transform.position.z);
    }

    void InputManager()
    {
        TouchGestures.SwipeGesture(leftSwipe, rightSwipe, upSwipe, downSwipe);

        //TouchGestures.RotateGesture(leftRotate, rightRotate);

        //TouchGestures.PinchGesture(zoomIn, zoomOut);

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
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        Sequence sequence = DOTween.Sequence();

        var initPositionY = transform.position.y;

        sequence.Append(transform.DOMoveY(transform.position.y / 3f, .5f));
        Wait(1f);
        sequence.Append(transform.DOMoveY(initPositionY, .5f));
    }

    private IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
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

        transform.DOMoveX(positions[(int)currentPosition], currentSideSpeed * Time.deltaTime);
        // We should add force instead of using DOTween until the position we need is reached
    }

    void IncreasePlayerSpeed()
    {
        currentSpeed = Mathf.SmoothStep(minSpeed, maxSpeed, time / accelerationTime);
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        currentSideSpeed = Mathf.SmoothStep(maxSideSpeed, minSideSpeed, time / accelerationTime);

        time += Time.deltaTime;
    }

    void CalculateDistanceTravelled()
    {
        distanceTravelled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        var roundedDistance = Mathf.RoundToInt(distanceTravelled);
        Debug.Log(roundedDistance);
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

public enum ScreenPosition
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