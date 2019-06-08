using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private enum Type
    {
        BOMBER, SUICIDER, SHOOTER
    }
    
    public static List<Rigidbody2D> enemies;

    [Header("Movement")]
    public float rotateSpeed = 4f;
    public float moveSpeed = 4f;
    public float maxGameDistance = 30f;
    
    //Repel
    public float repelRange = 3;
    public float repelForce = .5f;
    
    [Header("Shooter properties")]
    public float strafeDistance = 5f;

    [Header("Bomber properties")] 
    public float bombDistance = 0.5f;

    private bool stopToBomb = false;
    
    private Rigidbody2D rb;

    private Enemy m_enemy;

    // Start is called before the first frame update
    void Start()
    {
        m_enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        if (enemies == null)
        {
            enemies = new List<Rigidbody2D>();
        }
        enemies.Add(rb);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var distance = Vector2.Distance(rb.position, PlayerController.Position);

        //Rotate towards player (in 2D it's rotation in Z Axis)
        var direction = PlayerController.Position - rb.position;
        var step = rotateSpeed * Time.fixedDeltaTime;
        Vector2 rotateTowards = Vector3.RotateTowards(transform.up.normalized, direction.normalized, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, rotateTowards);

        if (stopToBomb || m_enemy.Type == Enemy.EnemyType.Bomber && distance <= bombDistance)
        {
            if (!stopToBomb)
            {
                GetComponent<EnemyBomber>().StartBombing();
                stopToBomb = true;
            }
            return;
        }

        if (distance > maxGameDistance)
        {
            Destroy(gameObject);
        }
        
        //If is shooter and near player then strafe?
        Vector2 newPos;
        if (m_enemy.Type == Enemy.EnemyType.Shooter && distance <= strafeDistance)
        {
            newPos = transform.position + transform.right * moveSpeed * Time.fixedDeltaTime;
        }
        else
        {
            //Calculate position (you are rotated so it's just moving forward) and move enemy there
            newPos = transform.position + transform.up * moveSpeed * Time.fixedDeltaTime;
            newPos += CalculateRepel();
        }
        
        rb.MovePosition(newPos);
    }

    private Vector2 CalculateRepel()
    {
        Vector2 repel = Vector2.zero;
        foreach (var enemy in enemies)
        {
            if (enemy == rb)
            {
                continue;
            }

            if (Vector2.Distance(enemy.position, rb.position) <= repelRange)
            {
                repel += (rb.position - enemy.position).normalized;
            }
        }

        return repel * Time.fixedDeltaTime * repelForce;
    }

    private void OnDestroy()
    {
        enemies?.Remove(rb);
    }
}
