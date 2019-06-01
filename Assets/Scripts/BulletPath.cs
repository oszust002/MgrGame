﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPath : MonoBehaviour
{
    public float speed = 10f;
    public bool isEnemyBullet;
    public int damage = 10;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        rb.velocity = transform.up * speed;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnemyBullet)
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }

            //Destroy if colliding with something else than enemy
            var playerController = other.GetComponent<Enemy>();
            if (playerController == null)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            var component = other.GetComponent<Enemy>();
            if (component != null)
            {
                component.TakeDamage(damage);
            }

            //Destroy if colliding with something else than player
            var playerController = other.GetComponent<PlayerController>();
            if (playerController == null)
            {
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
