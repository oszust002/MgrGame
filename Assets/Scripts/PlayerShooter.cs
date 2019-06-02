using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooter : MonoBehaviour
{
    public Transform fireStartPoint;
    public Weapon currentWeapon;

    public Image weaponImage;
    // Start is called before the first frame update
    
    private float m_NextShotTime = 0f;
    private AudioSource weaponAudio;

    private void Awake()
    {
        weaponAudio = gameObject.AddComponent<AudioSource>();
        weaponAudio.volume = 0.5f;
        weaponAudio.loop = false;
        weaponAudio.playOnAwake = false;
    }

    void Start()
    {
        UpdateWeaponAttributes();
    }

    private void UpdateWeaponAttributes()
    {
        if (currentWeapon != null && currentWeapon.image != null)
        {
            weaponImage.sprite = currentWeapon.image;
        }
        else
        {
            weaponImage.sprite = null;
        }

        weaponAudio.clip = currentWeapon.audio;
    }

    public void SetCurrentWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        UpdateWeaponAttributes();
    }

    // Update is called once per frame
    void Update()
    {
        if (Progress.instance.IsLevelLoading)
        {
            return;
        }
        
        if (Input.GetButton("Fire1"))
        {
            if (Time.time > m_NextShotTime)
            {
                currentWeapon.ShootBullet(fireStartPoint);
                weaponAudio.Play();
                m_NextShotTime = Time.time + 1f / currentWeapon.fireRate;
            }
        }
    }
}
