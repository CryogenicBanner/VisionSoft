using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwipeDetection : MonoBehaviour
{

    public bool detectSwipeOnlyAfterRelease = true;
    public float SWIPE_THRESHOLD = 20f;
    public Direction currentDirection;
    public bool activeDirection;

    private Vector2 fingerDown;
    private Vector2 fingerUp;

    private void Start()
    {
        activeDirection = false;
    }

    // Update is called once per frame
    void Update()
    {

        foreach (Touch touch in Input.touches)
        {
            SetActiveDirection(false);
            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;
            }

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                if (!detectSwipeOnlyAfterRelease)
                {
                    fingerDown = touch.position;
                    checkSwipe();
                }
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDown = touch.position;
                checkSwipe();
            }
        }

    }

    void checkSwipe()
    {
        //Check if Vertical swipe
        if (verticalMove() > SWIPE_THRESHOLD && verticalMove() > horizontalValMove())
        {
            //Debug.Log("Vertical");
            if (fingerDown.y - fingerUp.y > 0)//up swipe
            {
                OnSwipeUp();
                SetActiveDirection(true);
            }
            else if (fingerDown.y - fingerUp.y < 0)//Down swipe
            {
                OnSwipeDown();
                SetActiveDirection(true);
            }
            fingerUp = fingerDown;
        }

        //Check if Horizontal swipe
        else if (horizontalValMove() > SWIPE_THRESHOLD && horizontalValMove() > verticalMove())
        {
            //Debug.Log("Horizontal");
            if (fingerDown.x - fingerUp.x > 0)//Right swipe
            {
                OnSwipeRight();
                SetActiveDirection(true);
            }
            else if (fingerDown.x - fingerUp.x < 0)//Left swipe
            {
                OnSwipeLeft();
                SetActiveDirection(true);
            }
            fingerUp = fingerDown;
        }

        //No Movement at-all
        else
        {
            //Debug.Log("No Swipe!");
        }
    }

    float verticalMove(){ return Mathf.Abs(fingerDown.y - fingerUp.y); }
    float horizontalValMove() { return Mathf.Abs(fingerDown.x - fingerUp.x); }

    //////////////////////////////////CALLBACK FUNCTIONS/////////////////////////////
    void OnSwipeUp()
    {
        currentDirection = Direction.Up;
    }

    void OnSwipeDown()
    {
        currentDirection = Direction.Down;
    }

    void OnSwipeLeft()
    {
        currentDirection = Direction.Left;
    }

    void OnSwipeRight()
    {
        currentDirection = Direction.Right;
    }

    public void SetActiveDirection(bool active)
    {
        activeDirection = active;
    }
}