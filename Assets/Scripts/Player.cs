using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;

    /// <summary>
    /// Speed values
    /// </summary>
    float currentSpeed = 20f;
    float minSpeed;
    float maxSpeed = 60f;
    public float accelerationTime = 70f;
    float time;
    public float sideSpeed;

    public GameObject leftText;
    public GameObject rightText;

    float[] positions;

    [SerializeField]
    PlayerPosition currentPosition;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

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
    }

    void InputManager()
    {
        if(Input.GetKeyDown(KeyCode.A) || (Input.GetMouseButtonDown(0) && GetMouseButtonDownPosition() == ScreenPosition.Left))
        {
            leftText.SetActive(true);
            MoveToExactPosition(ScreenPosition.Left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || (Input.GetMouseButtonDown(0) && GetMouseButtonDownPosition() == ScreenPosition.Right))
        {
            rightText.SetActive(true);
            MoveToExactPosition(ScreenPosition.Right);
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetMouseButtonUp(0))
        {
            leftText.SetActive(false);
        }

        if (Input.GetKeyUp(KeyCode.D) || Input.GetMouseButtonUp(0))
        {
            rightText.SetActive(false);
        }
    }

    ScreenPosition GetMouseButtonDownPosition()
    {
        ScreenPosition screenPosition;

        if(Input.mousePosition.x > Screen.width / 2)
        {
            screenPosition = ScreenPosition.Right;
        }
        else
        {
            screenPosition = ScreenPosition.Left;
        }

        return screenPosition;
    }

    void SetInitialPosition()
    {
        currentPosition = PlayerPosition.Middle;

        transform.position = new Vector3(positions[(int)currentPosition], transform.position.y, transform.position.z);
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

        Debug.Log(currentSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Plataform"))
        {
            rb.velocity = Vector3.zero;
            currentSpeed = 0;
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