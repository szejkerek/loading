using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair_Movement : MonoBehaviour
{
    private SpriteRenderer sp;
    Vector2 vec;
    [Range(0,1)]
    public float MouseSensitivity = 0.1f;
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        sp.enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Camera.main != null)
        {
           vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        transform.position = Vector2.Lerp(transform.position, vec, MouseSensitivity);
    }
}
