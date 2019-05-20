using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public Transform fireStartPoint;
    public Weapon currentWeapon;
    // Start is called before the first frame update
    

    private float m_NextShotTime = 0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            if (Time.time > m_NextShotTime)
            {
                currentWeapon.ShootBullet(fireStartPoint);
                m_NextShotTime = Time.time + 1f / currentWeapon.fireRate;
            }
        }
    }
}
