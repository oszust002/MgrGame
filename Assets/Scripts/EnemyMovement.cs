using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isShooter;
    public float rotateSpeed = 4f;
    public float moveSpeed = 4f;
    public float maxGameDistance = 30f;
    public float strafeDistance = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isShooter = GetComponent<EnemyShooter>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var distance = Vector2.Distance(transform.position, PlayerController.Position);
        if (distance > maxGameDistance)
        {
            //Create new enemy? Or just destroy so it's easier for player?
            Destroy(gameObject);
        }
        //Rotate towards player (in 2D it's rotation in Z Axis)
        var direction = PlayerController.Position - rb.position;
        var step = rotateSpeed * Time.fixedDeltaTime;
        Vector2 rotateTowards = Vector3.RotateTowards(transform.up.normalized, direction.normalized, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, rotateTowards);
        
        //If is shooter and near player then strafe?
        Vector2 newPos;
        if (isShooter && distance <= strafeDistance)
        {
            newPos = transform.position + transform.right * moveSpeed * Time.fixedDeltaTime;
        }
        else
        {
            //Calculate position (you are rotated so it's just moving forward) and move enemy there
            newPos = transform.position + transform.up * moveSpeed * Time.fixedDeltaTime;
        }   
        rb.MovePosition(newPos);
        rb.MovePosition(newPos);
    }
}
