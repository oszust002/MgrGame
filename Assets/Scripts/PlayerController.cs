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

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        m_Movement.x = Input.GetAxisRaw("Horizontal");
        m_Movement.y = Input.GetAxisRaw("Vertical");
        m_MousePos = m_Camera.ScreenToWorldPoint(Input.mousePosition);
        Position = transform.position;
    }

    private void FixedUpdate()
    {
        Vector2 desiredVelocity = m_Movement * moveSpeed;
        rb.velocity = Vector2.SmoothDamp(rb.velocity, desiredVelocity, ref m_Velocity, moveSmooth);
        Vector2 lookDir = m_MousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    public void Die()
    {
        Debug.Log("Player Death");
        Destroy(gameObject);
    }
}
