using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static Vector2 Position = Vector2.zero;
    
    public float moveSpeed = 10f;
    public float moveSmooth = .3f;

    private Vector2 m_Movement = Vector2.zero;
    private Vector2 m_Velocity = Vector2.zero;
    private Vector2 m_MousePos = Vector2.zero;

    private Camera m_Camera;
    private Rigidbody2D rb;
    private bool isJoystickControl;
    
    private float xDir;
    private float yDIr;
    

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
                
        //Check if there is joystick connected
        var joystickNames = Input.GetJoystickNames();
        isJoystickControl = joystickNames.Length > 0 && joystickNames[0].Length > 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Movement
        m_Movement.x = Input.GetAxisRaw("Horizontal");
        m_Movement.y = Input.GetAxisRaw("Vertical");
        
        //Direction by joystick
        xDir = Input.GetAxisRaw("HorizontalDS");
        yDIr = Input.GetAxisRaw("VerticalDS");
        
        //Mouse position for direction by mouse
        m_MousePos = m_Camera.ScreenToWorldPoint(Input.mousePosition);
        Position = transform.position;
    }

    private void FixedUpdate()
    {
        Vector2 desiredVelocity = m_Movement * moveSpeed;
        rb.velocity = Vector2.SmoothDamp(rb.velocity, desiredVelocity, ref m_Velocity, moveSmooth);
        Vector2 lookDir;
        if (isJoystickControl)
        {
            lookDir = new Vector2(xDir, yDIr);
        }
        else
        {
            lookDir = m_MousePos - rb.position;
        }

        if (!isJoystickControl || Math.Abs(lookDir.x) > 0.01 || Math.Abs(lookDir.y) > 0.01)
        {
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }
    }

    public void Die()
    {
        Debug.Log("Player Death");
        Destroy(gameObject);
        GameManager.instance.PlayerDied();
    }
}
