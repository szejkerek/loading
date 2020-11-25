using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cam_Movement : MonoBehaviour
{
    Transform crosshairPos;
    Transform targetPos;

    [Header("CAM PROPORTIES")]
    [Range(0.01f,20f)]
    public float smoothing_inside;
    [Range(0.01f, 20f)]
    public float smoothing_outside;
    [Header("Big one = RED")]
    [Range(0.01f, 1f)]
    public float percentageBigRect;
    [Header("Small one = BLUE")]
    [Range(0.01f, 1f)]
    public float percentageSmallRect;
    [Header("SCREEN SIZE PROBLEM")]
    [Range(0.01f, 200f)]
    public float screenAspectRatio = 1;
    //normalized screen sizes
    Vector3 _screenSize;

    void DrawRectangle(Vector2 targetPos, Vector2 screenSize, float percentage,Color color)
    {
        screenSize *= percentage;
     
        //Create corners
        Vector3 left_top = new Vector2(targetPos.x - screenSize.x, targetPos.y + screenSize.y);
        Vector3 left_bottom = new Vector2(targetPos.x - screenSize.x, targetPos.y - screenSize.y);
        Vector3 right_top = new Vector2(targetPos.x + screenSize.x, targetPos.y + screenSize.y);
        Vector3 right_bottom = new Vector2(targetPos.x + screenSize.x, targetPos.y - screenSize.y);

        //Draw rectangle relative to game units
        Debug.DrawLine(right_top, right_bottom, color);;
        Debug.DrawLine(right_top, left_top, color);
        Debug.DrawLine(left_top, left_bottom, color);
        Debug.DrawLine(left_bottom, right_bottom, color);
    }

    bool isInsideRect( Vector2 crosshairPos, Vector2 targetPos, Vector2 screenSize, float percentage)
    {
        screenSize *= percentage;
        if(crosshairPos.y > targetPos.y + (screenSize.y))
        {
            //Debug.Log("Top");
            return false;
        }else if(crosshairPos.y < targetPos.y - (screenSize.y ))
        {
            //Debug.Log("Down");
            return false;
        }
        else if(crosshairPos.x > targetPos.x + (screenSize.x))
        {
            //Debug.Log("Right");
            return false;
        }
        else if(crosshairPos.x < targetPos.x - (screenSize.x ))
        {
            //Debug.Log("Left");
            return false;
        }
        else
        {
            return true;
        }   
    }
    void MoveCam(Vector2 destination,float speed)
    {
        if(Camera.main != null)
        {
            Vector3 temp_pos;
            temp_pos = Vector2.Lerp(Camera.main.transform.position, destination, speed*Time.deltaTime);
            temp_pos.z = -10;
            Camera.main.transform.position = temp_pos;
        }
        
    }
    Vector2 MaxDesiredPosition(Vector2 crosshairPos, Vector2 targetPos, Vector2 screenSize, float percentageBig)
    {
        Vector2 desiredPosition = new Vector2(0f,0f);
        screenSize *= percentageBig;
        if ((crosshairPos.y >= targetPos.y + screenSize.y))
        {
            if (crosshairPos.x >= targetPos.x - screenSize.x && crosshairPos.x <= targetPos.x + screenSize.x)
            {
                //Debug.Log("Góra środek");
                desiredPosition = new Vector3(crosshairPos.x, targetPos.y + screenSize.y);
            }
            else if (crosshairPos.x < targetPos.x - screenSize.x)
            {
                // Debug.Log("Góra lewo");
                desiredPosition = new Vector3(targetPos.x - screenSize.x, targetPos.y + screenSize.y);
            }
            else if (crosshairPos.x > targetPos.x + screenSize.x)
            {
                // Debug.Log("Góra prawo");
                desiredPosition = new Vector3(targetPos.x + screenSize.x, targetPos.y + screenSize.y);
            }
            return (desiredPosition + targetPos) / 2;
        }
        if (crosshairPos.y <= targetPos.y - screenSize.y)
        {
            if (crosshairPos.x >= targetPos.x - screenSize.x && crosshairPos.x <= targetPos.x + screenSize.x)
            {
                // Debug.Log("Dół środek");
                desiredPosition = new Vector3(crosshairPos.x, targetPos.y - screenSize.y);
            }
            else if (crosshairPos.x < targetPos.x - screenSize.x)
            {
                // Debug.Log("Dół lewo");
                desiredPosition = new Vector3(targetPos.x - screenSize.x, targetPos.y - screenSize.y);
            }
            else if (crosshairPos.x > targetPos.x + screenSize.x)
            {
                //Debug.Log("Dół prawo");
                desiredPosition = new Vector3(targetPos.x + screenSize.x, targetPos.y - screenSize.y);
            }
            return (desiredPosition + targetPos) / 2;
        }
        if (crosshairPos.x >= targetPos.x + screenSize.x)
        {
            if (crosshairPos.y >= targetPos.y - screenSize.y && crosshairPos.y <= targetPos.y + screenSize.y)
            {
                //Debug.Log("Prawo środek");
                desiredPosition = new Vector3(targetPos.x + screenSize.x, crosshairPos.y);
            }
            return (desiredPosition + targetPos) / 2;
        }
        if (crosshairPos.x <= targetPos.x - screenSize.x)
        {
            if (crosshairPos.y >= targetPos.y - screenSize.y && crosshairPos.y <= targetPos.y + screenSize.y)
            {
                //Debug.Log("Lewo środek");
                desiredPosition = new Vector3(targetPos.x - screenSize.x, crosshairPos.y);
            }
            return (desiredPosition + targetPos)/2;
        }
        else
        {
            return desiredPosition;
        }

    }


    void Awake()
    {
        GameObject crosshair = GameObject.FindGameObjectWithTag("Crosshair");
        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (crosshair != null)
        {
            crosshairPos = crosshair.GetComponent<Transform>();
           
        }
        if (target != null)
        {
            targetPos = target.GetComponent<Transform>();
        }
    }

    private void Start()
    {
        
        _screenSize.x = 1 * (float)Screen.width/100;
        _screenSize.y = 1  * (float)Screen.height  /100;
    }
    void FixedUpdate()
    {
        //if (Camera.main != null)
        //{
        //    //current screenSize
        //    _screenSize = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height);
        //    Debug.Log(Screen.currentResolution);
        //}

        _screenSize.x = 1 * (float)Screen.width / screenAspectRatio;
        _screenSize.y = 1 * (float)Screen.height / screenAspectRatio;


        DrawRectangle(targetPos.position, _screenSize, percentageBigRect, Color.red);
        DrawRectangle(targetPos.position, _screenSize, percentageSmallRect, Color.blue);
        Vector2 desired_position = (targetPos.position + crosshairPos.position)/2;
        if (isInsideRect(crosshairPos.position, targetPos.position, _screenSize, percentageBigRect))
        {
            if (isInsideRect(crosshairPos.position, targetPos.position, _screenSize, percentageSmallRect))
            {
                ///Inside Small Rect
                //Debug.Log("Inside Small Rect");
                MoveCam(targetPos.position, smoothing_inside);

            }
            else
            {
                ///Inside Big Rect && Outside Small Rect

                //Debug.Log("Inside Big Rect && Outside Small Rect");
                MoveCam(desired_position, smoothing_outside);
            }
        }
        else
        {
            desired_position = MaxDesiredPosition(crosshairPos.position, targetPos.position, _screenSize, percentageBigRect);
            MoveCam(desired_position, smoothing_outside);
        }
      
        
    }
}
