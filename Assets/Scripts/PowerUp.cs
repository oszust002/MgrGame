using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public Weapon powerUpWeapon;
    // Start is called before the first frame update


    private void Update()
    {
        transform.Rotate(0f, 0f, Time.fixedDeltaTime * 50f, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var playerShooter = other.GetComponent<PlayerShooter>();
        if (playerShooter == null) return;
        playerShooter.currentWeapon = powerUpWeapon;
        Destroy(gameObject);
    }
}
