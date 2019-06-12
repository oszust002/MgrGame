using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooter : MonoBehaviour
{
    public Transform fireStartPoint;
    public Weapon currentWeapon;

    public Image weaponImage;
    public Image specialPowerImage;
    // Start is called before the first frame update
    public GameObject specialPowerPrefab;
    public float specialDelay = 10f;
    
    private float m_NextShotTime = 0f;
    private AudioSource weaponAudio;

    private void Awake()
    {
        weaponAudio = gameObject.AddComponent<AudioSource>();
        weaponAudio.volume = 0.3f;
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
        if (Progress.instance.IsLevelLoading || GameManager.instance.gamePaused)
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

        specialPowerImage.fillAmount =
            Mathf.Clamp(specialPowerImage.fillAmount + 1 / specialDelay * Time.fixedDeltaTime, 0, 1);
        //if fire or clashing fist, apply special power
        if (Input.GetButton("Fire2"))
        {
            if (specialPowerImage.fillAmount >= 1)
            {
                Instantiate(specialPowerPrefab, transform.position, transform.rotation);
                specialPowerImage.fillAmount = 0;
            }    
        }
    }
}
