using UnityEngine;
using System.Collections;

public class SwipeControl : MonoBehaviour
{

    public static System.Action<SwipeDirection> OnSwipe;

    public enum SwipeDirection
    {
        Null = 0,
        //no swipe detected
        Duck = 1,
        //swipe down detected
        Jump = 2,
        //swipe up detected
        Right = 3,
        //swipe right detected
        Left = 4
        //swipe left detected
    }


    //Constants
    public float fSensitivity = 15f;

    //VARIABLES
    //distance calculation
    private float fInitialX;
    private float fInitialY;
    private float fFinalX;
    private float fFinalY;

    private float inputX;
    private float inputY;
    //y-coordinate
    private float slope;
    //slope (m) of the the
    private float fDistance;
    //magnitude of distance between two positions
    private int iTouchStateFlag;
    //flag to check
    SwipeDirection sSwipeDirection;
    //string to receive swipe output


    void Update()
    {


        if (Input.mousePosition.y < Screen.height - Screen.height / 8 && Input.mousePosition.y > Screen.height - Screen.height)
        {
            if (iTouchStateFlag == 0 && Input.GetMouseButtonDown(0))
            {    //state 1 of swipe control
                fInitialX = Input.mousePosition.x;  //get the initial x mouse/ finger value
                fInitialY = Input.mousePosition.y;  //get the initial y mouse/ finger value

                sSwipeDirection = SwipeDirection.Null;
                iTouchStateFlag = 1;
            }
            if (iTouchStateFlag == 1)
            {   //state 2 of swipe control
                fFinalX = Input.mousePosition.x;
                fFinalY = Input.mousePosition.y;

                sSwipeDirection = swipeDirection(); //get the swipe direction
                if (sSwipeDirection != SwipeDirection.Null)
                    iTouchStateFlag = 2;
            }//end of state 1		
            if (iTouchStateFlag == 2 || Input.GetMouseButtonUp(0))
            {  //state 3 of swipe control
                iTouchStateFlag = 0;
            }

        }

        if (OnSwipe != null)
            OnSwipe(getSwipeDirection());

    }

    /*
    *	FUNCTION: Return swipe direction.
    *	RETURNS: Returns NULL if no swipes are detected.
    *			  Returns SwipeDirection if a swipe is detected
    */
    public SwipeDirection getSwipeDirection()
    {
        if (sSwipeDirection != SwipeDirection.Null)
        {//if a swipe is detected
            var etempSwipeDirection = sSwipeDirection;
            sSwipeDirection = SwipeDirection.Null;

            return etempSwipeDirection;
        }
        else
            return SwipeDirection.Null;//if no swipe was detected
    }

    /*
    *	FUNCTION: Calculate the swipe direction
    */
    private SwipeDirection swipeDirection()
    {
        //calculate the slope of the swipe
        inputX = fFinalX - fInitialX;
        inputY = fFinalY - fInitialY;
        slope = inputY / inputX;
        //calculate the distance of tap start and end
        fDistance = Mathf.Sqrt(Mathf.Pow((fFinalY - fInitialY), 2) + Mathf.Pow((fFinalX - fInitialX), 2));

        if (fDistance <= fSensitivity)//higher the dividing factor lower the sensitivity
            return SwipeDirection.Null;

        if (inputX >= 0 && inputY > 0 && slope > 1)
        {//first octant JUMP
            return SwipeDirection.Jump;
        }
        else if (inputX <= 0 && inputY > 0 && slope < -1)
        {//eighth octant  JUMP
            return SwipeDirection.Jump;
        }
        else if (inputX > 0 && inputY >= 0 && slope < 1 && slope >= 0)
        {//second octant  RIGHT
            return SwipeDirection.Right;

        }
        else if (inputX > 0 && inputY <= 0 && slope > -1 && slope <= 0)
        {//third octant  RIGHT
            return SwipeDirection.Right;
        }
        else if (inputX < 0 && inputY >= 0 && slope > -1 && slope <= 0)
        {//seventh octant  LEFT
            return SwipeDirection.Left;
        }
        else if (inputX < 0 && inputY <= 0 && slope >= 0 && slope < 1)
        {//sixth octant  LEFT
            return SwipeDirection.Left;
        }
        else if (inputX >= 0 && inputY < 0 && slope < -1)
        {//fourth octant  DUCK
            return SwipeDirection.Duck;
        }
        else if (inputX <= 0 && inputY < 0 && slope > 1)
        {//fifth octant  DUCK
            return SwipeDirection.Duck;
        }//end of else if

        return SwipeDirection.Null;
    }
    //end of SwipeDirection function
}
