using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair_Movement : MonoBehaviour
{
    private SpriteRenderer sp;
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        sp.enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = vec;
    }
}
