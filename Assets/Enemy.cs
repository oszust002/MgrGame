using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnCollisionEnter2D(Collision2D other)
    {
        var playerController = other.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.Die();
            Debug.Log("Enemy death"); //Death logic (animations etc)
            Destroy(gameObject);
        }
    }
}
