using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
   
    [Header("CAMERA PROPORTIES")]

    [Range(0.0f, 100f)]
    public float smoothingInside;
    [Range(0.0f, 100f)]
    public float smoothingOutside;
    [Header("Percent of screen to activate camera movement.")]
    [Range(0.0f, 200f)]
    public float border_limit;
    float ratio = 87.7f;
    Transform target;
    public Transform cam;

    [Header("Camera Lock Delta")]
    [Range(0f, 1f)]
    public float cam_lock;

    bool cam_static = true;

    

    void Awake()
    {
        
        GameObject player = GameObject.FindWithTag("Player");
        if (player!=null)
        {
            target = player.GetComponent<Transform>();
        }
    }
    bool MouseOnScreenBoundries(Vector3 mouse_input, Vector3 target, float local_weight, float local_height)
    {

        if ((mouse_input.y >= target.y + local_height)    )
        {
            return true;
        }else if(mouse_input.y <= target.y - local_height)
        {
            return true;
        }else if (mouse_input.x >= target.x + local_weight)
        {
            return true;
        }else if (mouse_input.x <= target.x - local_weight)
        {
            return true;
        }
        else
        {
            return false;
        }    


    }
    Vector3 MouseOutsideScreen(Vector3 mouse_input, Vector3 target, float local_width, float local_height)
    {
        Vector3 temp_point = new Vector3(0f,0f,0f);
        if ((mouse_input.y >= target.y + local_height))
        {
            if(mouse_input.x>= target.x - local_width && mouse_input.x <= target.x + local_width)
            {
                //Debug.Log("Góra środek");
                temp_point = new Vector3(mouse_input.x, target.y + local_height, 0);
            }
            else if(mouse_input.x < target.x - local_width)
            {
               // Debug.Log("Góra lewo");
                temp_point = new Vector3(target.x - local_width, target.y + local_height, 0);
            }
            else if (mouse_input.x > target.x + local_width)
            {
               // Debug.Log("Góra prawo");
                temp_point = new Vector3(target.x + local_width, target.y + local_height, 0);
            }
            return temp_point;
        }
         if (mouse_input.y <= target.y - local_height)
        {
            if (mouse_input.x >= target.x - local_width && mouse_input.x <= target.x + local_width)
            {
               // Debug.Log("Dół środek");
                temp_point = new Vector3(mouse_input.x, target.y - local_height, 0);
            }
            else if (mouse_input.x < target.x - local_width)
            {
               // Debug.Log("Dół lewo");
                temp_point = new Vector3(target.x - local_width, target.y - local_height, 0);
            }
            else if (mouse_input.x > target.x + local_width)
            {
                //Debug.Log("Dół prawo");
                temp_point = new Vector3(target.x + local_width, target.y - local_height, 0);
            }
            return temp_point;
        }
         if (mouse_input.x >= target.x + local_width)
        {
            if (mouse_input.y >= target.y - local_height && mouse_input.y <= target.y + local_height)
            {
                //Debug.Log("Prawo środek");
                temp_point = new Vector3(target.x + local_width, mouse_input.y, 0);
            }
            return temp_point;
        }
         if (mouse_input.x <= target.x - local_width)
        {
            if (mouse_input.y >= target.y - local_height && mouse_input.y <= target.y + local_height)
            {
                //Debug.Log("Lewo środek");
                temp_point = new Vector3(target.x - local_width, mouse_input.y, 0);
            }
            return temp_point;
        }
        else
        {
            return temp_point;
        }


    }

    void DrawRectangle(Vector3 target,float local_weight, float local_height)
    {

        Vector3 left_top =      new Vector3(target.x - local_weight, target.y + local_height, 0f);
        Vector3 left_bottom =   new Vector3(target.x - local_weight, target.y - local_height, 0f);

        Vector3 right_top =     new Vector3(target.x + local_weight, target.y + local_height, 0f);
        Vector3 right_bottom =  new Vector3(target.x + local_weight, target.y - local_height, 0f);

       

        Debug.DrawLine(right_top, right_bottom);
        Debug.DrawLine(right_top, left_top );

        Debug.DrawLine(left_top, left_bottom);
        Debug.DrawLine(left_bottom, right_bottom);



        Debug.DrawLine(right_top, target);
        Debug.DrawLine(right_bottom, target);
        Debug.DrawLine(left_top, target);
        Debug.DrawLine(left_bottom, target);
    }



    void FixedUpdate()
    {
        float normalized_border = border_limit / 100;
        float noramlized_height = (Screen.height / 2) * normalized_border;
        float noramlized_width = (Screen.width / 2) * normalized_border;

       
        float local_width = noramlized_width/ratio;
        float local_height = noramlized_height / ratio;

        float local_width_outside = Screen.width/2   / ratio;
        float local_height_outside = Screen.height/2  / ratio;


        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPosition = target.position;



        DrawRectangle(targetPosition,local_width,local_height );
        DrawRectangle(targetPosition, local_width_outside, local_height_outside);

        if (MouseOnScreenBoundries(mouse, targetPosition, local_width, local_height ))
        {
            cam_static = false;


            Vector3 middlePoint = (mouse + targetPosition) / 2;
            if (!MouseOnScreenBoundries(mouse, targetPosition, local_width_outside, local_height_outside))
            {
                middlePoint = (mouse + targetPosition) / 2;
                Vector3 newPos = Vector3.Lerp(cam.position, middlePoint, smoothingOutside * Time.deltaTime);
                newPos.z = -10;
                cam.position = newPos;
            }
            else
            {
                
                Vector3 temp_mouse = MouseOutsideScreen(mouse, targetPosition, local_width_outside, local_height_outside);
                middlePoint = (temp_mouse + targetPosition) / 2;
                Vector3 newPos = Vector3.Lerp(cam.position, middlePoint, smoothingOutside * Time.deltaTime);
                newPos.z = -10;
                cam.position = newPos;
            }
           
            


        }
        else
        {
            if(cam_static == false)
            {
                if (Vector2.Distance(cam.position, targetPosition) >= cam_lock)
                {
                    Vector3 newPos = Vector3.Lerp(cam.position, targetPosition, smoothingInside*Time.deltaTime);
                    newPos.z = -10;
                    cam.position = newPos;
                    if (Vector2.Distance(cam.position, targetPosition) < cam_lock)
                    {
                        cam_static = true;
                    }
                }
            }
            else
            {
                Vector3 staticPos = targetPosition;
                staticPos.z = -10;
                cam.position = staticPos;
            }
           
        }


    }
}
