using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float sideSpeed;
    public GameObject leftText;
    public GameObject rightText;

    float[] positions;

    [SerializeField]
    PlayerPosition currentPosition;

    float fraction = 0;

    // Start is called before the first frame update
    void Start()
    {
        float leftPositionX = -5f;
        float middlePositionX = 0f;
        float rightPositionX = 5f;

        positions = new float[] { leftPositionX, middlePositionX, rightPositionX };

        SetInitialPosition();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed * Time.deltaTime);

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