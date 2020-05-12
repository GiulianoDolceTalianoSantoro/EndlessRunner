using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchGestures : MonoBehaviour
{
    static Touch touch;
    static Vector3 beginTouchPosition;
    static Vector3 endTouchPosition;

    static bool rotating = false;
    static float rotGestureWidth = 1;
    static float rotAngleMinimum = 1;

    static bool pinching = false;

    public static int speed = 4; 
    public static Camera selectedCamera; 
    public static float MINSCALE = 2.0F; 
    public static float MAXSCALE = 5.0F; 
    public static float minPinchSpeed = 5.0F; 
    public static float varianceInDistances = 5.0F; 
    private static float touchDelta = 0.0F; 
    private static Vector2 previousDistance = new Vector2(0, 0); 
    private static Vector2 currentDistance = new Vector2(0, 0); 
    private static float speedTouch0 = 0.0F; 
    private static float speedTouch1 = 0.0F;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SwipeGesture(Action leftMovementAction, Action rightMovementAction, Action upMovementAction)
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
                        leftMovementAction();
                    }
                    else if (beginTouchPosition.x + 100f < endTouchPosition.x)
                    {
                        rightMovementAction();
                    }
                    else if (beginTouchPosition.y < endTouchPosition.y)
                    {
                        upMovementAction();
                    }

                    break;
            }
        }
    }

    public static void RotateGesture(Action leftRotationAction, Action rightRotationAction)
    {
        if (Input.touchCount == 2 && !pinching)
        {
            if (!rotating)
            {
                beginTouchPosition = Input.GetTouch(1).position - Input.GetTouch(0).position;
                rotating = beginTouchPosition.sqrMagnitude > rotGestureWidth * rotGestureWidth;
            }
            else
            {
                var currentTouchPosition = Input.GetTouch(1).position - Input.GetTouch(0).position;
                var angleOffset = Vector2.Angle(beginTouchPosition, currentTouchPosition);
                var LR = Vector3.Cross(beginTouchPosition, currentTouchPosition);

                if (angleOffset > rotAngleMinimum)
                {
                    if (LR.z > 0)
                    {
                        leftRotationAction();
                    }
                    else if (LR.z < 0)
                    {
                        rightRotationAction();
                    }
                }
            }
        }
        else
        {
            rotating = false;
        }
    }

    public static void PinchGesture(Action zoomInAction, Action zoomOutAction)
    {
        if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved && !rotating)
        {
            currentDistance = Input.GetTouch(0).position - Input.GetTouch(1).position;

            previousDistance = ((Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition) - (Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition));

            touchDelta = currentDistance.magnitude - previousDistance.magnitude;
            speedTouch0 = Input.GetTouch(0).deltaPosition.magnitude / Input.GetTouch(0).deltaTime;
            speedTouch1 = Input.GetTouch(1).deltaPosition.magnitude / Input.GetTouch(1).deltaTime;

            if ((touchDelta + varianceInDistances <= 1) && (speedTouch0 > minPinchSpeed) && (speedTouch1 > minPinchSpeed))
            {
                zoomOutAction();
            }

            if ((touchDelta + varianceInDistances > 1) && (speedTouch0 > minPinchSpeed) && (speedTouch1 > minPinchSpeed))
            {
                zoomInAction();
            }
        }
    }
}
