using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarfieldController : MonoBehaviour
{
    public int maxStars = 100;
    public float starSize = 0.1f;
    public float StarSizeRange = 0.5f;
    public float FieldWidth = 20f;
    public float FieldHeight = 25f;
    public bool Colorize = false;

    ParticleSystem Particles;
    ParticleSystem.Particle[] Stars;

    private float xOffset;
    private float yOffset;
    private Transform theCamera;
    [Range(-0.5f, 0.5f)] public float horizontalStarSpeed;
    [Range(-0.5f, 0.5f)] public float verticalStarSpeed;

    // Start is called before the first frame update
    private void Awake()
    {
        Stars = new ParticleSystem.Particle[maxStars];
        Particles = GetComponent<ParticleSystem>();

        xOffset = FieldWidth * 0.5f; // Offset the coordinates to distribute the spread
        yOffset = FieldHeight * 0.5f; // around the object's center

        for (var i = 0; i < maxStars; i++)
        {
            var randSize = Random.Range(StarSizeRange, StarSizeRange + 1f); // Randomize star size within parameters
            var scaledColor =
                Colorize ? randSize - StarSizeRange : 1f; // If coloration is desired, color based on size

            Stars[i].position = GetRandomInRectangle(FieldWidth, FieldHeight) + transform.position;
            Stars[i].startSize = starSize * randSize;
            Stars[i].startColor = new Color(1f, scaledColor, scaledColor, 1f);
        }

        Particles.SetParticles(Stars, Stars.Length); // Write data to the particle system
    }

    private void Start()
    {
        theCamera = Camera.main.transform;
    }

    private Vector3 GetRandomInRectangle(float width, float height)
    {
        var x = Random.Range(0, width);
        var y = Random.Range(0, height);
        return new Vector3(x - xOffset, y - yOffset, 0);
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.gamePaused)
        {
            return;
        }

        for (var i = 0; i < maxStars; i++)
        {
            var pos = Stars[i].position;

            if (pos.x < theCamera.position.x - xOffset)
            {
                pos.x += FieldWidth;
            }
            else if (pos.x > theCamera.position.x + xOffset)
            {
                pos.x -= FieldWidth;
            }

            if (pos.y < theCamera.position.y - yOffset)
            {
                pos.y += FieldHeight;
            }
            else if (pos.y > theCamera.position.y + yOffset)
            {
                pos.y -= FieldHeight;
            }

            Stars[i].position = pos + Vector3.right * horizontalStarSpeed + Vector3.up * verticalStarSpeed;
        }

        Particles.SetParticles(Stars, Stars.Length);
    }
}