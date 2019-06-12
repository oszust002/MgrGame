using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPower : MonoBehaviour
{
    public float timeOfPower = 2f;

    public float speed = 1f;
    public float rotationSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeOfPower);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var component = other.GetComponent<Enemy>();
        if (component != null)
        {
            component.Die();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gamePaused)
        {
            return;
        }
        transform.localScale += new Vector3(speed * Time.fixedDeltaTime, speed * Time.fixedDeltaTime, 0);
           transform.Rotate(0, 0, rotationSpeed*Time.fixedDeltaTime);
    }
}