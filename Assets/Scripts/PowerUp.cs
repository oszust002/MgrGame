using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    
    // Start is called before the first frame update


    private void Update()
    {
        if (GameManager.instance.gamePaused)
        {
            return;
        }
        var distance = Vector3.Distance(transform.position, PlayerController.Position);
        if (distance > 40f)
        {
            Destroy(gameObject);
        }
        transform.Rotate(0f, 0f, Time.fixedDeltaTime * 50f, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        ApplyPowerUp(other);
        Destroy(gameObject);
    }

    protected abstract void ApplyPowerUp(Collider2D other);
}
