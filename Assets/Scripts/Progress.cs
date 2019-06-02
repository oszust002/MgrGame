using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{
    public static Progress instance;

    public List<Level> levels;
    public EnemySpawner enemySpawner;

    [Header("UI")]
    public TextMeshProUGUI textScore;
    public Slider slider;
    
    [HideInInspector]
    public bool IsLevelLoading;
    private float m_Score;
    private int currentLevel;
    
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        m_Score = 0;
        LoadLevel(0);
        UpdateScoreUI();
    }

    private void LoadLevel(int number)
    {
        enemySpawner.enemies = levels[number].enemies;
        enemySpawner.spawnTime = levels[number].spawnRate;
        
        if (number < levels.Count - 1)
        {
            slider.minValue = m_Score;
            slider.maxValue = levels[number].finishScore;
        }
        currentLevel = number;
    }

    public void AddScore(int amount)
    {
        m_Score += amount;
        if (m_Score >= levels[currentLevel].finishScore)
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                var enemyComponent = enemy.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.Remove();
                }
                else
                {
                    Destroy(enemy);
                }
            }
            var bullets = GameObject.FindGameObjectsWithTag("Bullet");
            foreach (var bullet in bullets)
            {
                var bulletPath = bullet.GetComponent<BulletPath>();
                if (bulletPath != null)
                {
                    bulletPath.Remove();
                }
                else
                {
                    Destroy(bullet);
                }
            }
            
            if (levels[currentLevel].endTheGame || currentLevel > levels.Count - 1)
            {
                StartCoroutine(EndTheGame());
            }
            else
            {
                StartCoroutine(LevelUp());
                LoadLevel(currentLevel + 1);
            }
            
        }
        UpdateScoreUI();
    }

    private IEnumerator EndTheGame()
    {
        IsLevelLoading = true;
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator LevelUp()
    {
        IsLevelLoading = true;
        yield return new WaitForSeconds(2f);
        
        IsLevelLoading = false;
    }

    private void UpdateScoreUI()
    {
        textScore.text = m_Score.ToString();
        slider.value = m_Score;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
