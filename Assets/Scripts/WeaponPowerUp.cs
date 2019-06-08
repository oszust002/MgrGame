using UnityEngine;

public class WeaponPowerUp : PowerUp
{
    public Weapon powerUpWeapon;
    protected override void ApplyPowerUp(Collider2D other)
    {
        var playerShooter = other.GetComponent<PlayerShooter>();
        if (playerShooter == null) return;
        playerShooter.SetCurrentWeapon(powerUpWeapon);
        
    }
}