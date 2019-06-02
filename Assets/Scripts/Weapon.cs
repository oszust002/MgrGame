using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Customs/Weapon")]
public class Weapon : ScriptableObject
{
    public GameObject shotPrefab;
    public Sprite image;

    public float fireRate;
    // Start is called before the first frame update
    public void ShootBullet(Transform startPoint)
    {
        var shotInstance = Instantiate(shotPrefab,startPoint.position, startPoint.rotation);
        Destroy(shotInstance, 10f);
    }
}
