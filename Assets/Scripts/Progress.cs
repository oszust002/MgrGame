using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{
    public static Progress instance;

    public List<Level> levels;
    public EnemySpawner enemySpawner;
    
    [Header("Hard Mode options")]
    public float hardModeTimeThreshold = 3f;
    public int destroyedEnemiesThreshold = 3;
    public float hardModeTime = 10f;
    private HardModeHandler m_HardModeHandler;
    private float m_HardModeStartTime = 0f;

    [Header("UI")]
    public TextMeshProUGUI textScore;
    public Slider slider;
    public Animator endGameAnimation;
    
    [HideInInspector]
    public bool IsLevelLoading;
    private float m_Score;
    private int currentLevel;
    private Player m_Player;
    

    // Start is called before the first frame update
    void Start()
    {
        m_HardModeHandler = new HardModeHandler();
        m_Player = FindObjectOfType<Player>();
        if (instance == null)
        {
            instance = this;
        }

        m_Score = 0;
        LoadLevel(0);
        UpdateScoreUI();
        if (AffectiveManager.instance.AffectiveEnabled())
        {
            AffectiveManager.instance.emotionManager.onNewEmotion += HandleEmotions;
        }
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
            m_HardModeHandler.Disable();
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
        //If not in affective mode, 
        if (!AffectiveManager.instance.AffectiveEnabled()) {
            CheckHardMode();
        }
        UpdateScoreUI();
    }

    private void CheckHardMode()
    {
        
        if (!m_HardModeHandler.hardModeEnabled)
        {
            m_HardModeHandler.Tick();
            if (!m_HardModeHandler.hardModeEnabled) return;
            m_HardModeStartTime = Time.time;
            enemySpawner.enemies = levels[currentLevel].specialEnemies;
            enemySpawner.spawnTime = levels[currentLevel].spawnRate / 2;
        }
    }

    private IEnumerator EndTheGame()
    {
        IsLevelLoading = true;
        endGameAnimation.SetTrigger("EndGame");
        yield return new WaitForSeconds(4f);
        GameManager.instance.gameEnded = true;
    }

    private IEnumerator LevelUp()
    {
        ApplyReward();
        IsLevelLoading = true;
        yield return new WaitForSeconds(2f);
        
        IsLevelLoading = false;
    }

    private void ApplyReward()
    {
        if (m_Player != null)
        {
            m_Player.SetMaxHealth(m_Player.maxHealth + levels[currentLevel].healthBonusReward);
        }
    }

    private void UpdateScoreUI()
    {
        textScore.text = m_Score.ToString();
        slider.value = m_Score;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_HardModeHandler.hardModeEnabled && Time.time - m_HardModeStartTime >= hardModeTime)
        {
            enemySpawner.enemies = levels[currentLevel].enemies;
            enemySpawner.spawnTime = levels[currentLevel].spawnRate;
            m_HardModeHandler.Disable();
        }
    }

    private void HandleEmotions(Emotion previousEmotion, Emotion emotion)
    {
        if (emotion.Equals(Emotion.Neutral) || emotion.Equals(Emotion.Relaxed))
        {
            m_HardModeHandler.Enable();
            m_HardModeStartTime = Time.time;
            enemySpawner.enemies = levels[currentLevel].specialEnemies;
            enemySpawner.spawnTime = levels[currentLevel].spawnRate / 2;
        } else if (emotion.Equals(Emotion.Angry))
        {
            enemySpawner.enemies = levels[currentLevel].enemies;
            enemySpawner.spawnTime = levels[currentLevel].spawnRate;
            m_HardModeHandler.Disable();
        }
    }

    private void OnDestroy()
    {
        AffectiveManager.instance.emotionManager.onNewEmotion -= HandleEmotions;
    }
}

internal class HardModeHandler
{
    private float m_HardModeCountTime;
    private int m_DestroyedEnemies;
    public bool hardModeEnabled;
    
    public void Tick()
    {
        if (hardModeEnabled)
        {
            return;
        }
        if (m_DestroyedEnemies == 0)
        {
            m_DestroyedEnemies++;
            m_HardModeCountTime = Time.time;
        }
        else if (Time.time - m_HardModeCountTime < Progress.instance.hardModeTimeThreshold)
        {
            m_DestroyedEnemies++;
            if (m_DestroyedEnemies == Progress.instance.destroyedEnemiesThreshold)
            {
                hardModeEnabled = true;
            }
        }
        else
        {
            m_DestroyedEnemies = 0;
            m_HardModeCountTime = 0f;
        }
    }

    public void Enable()
    {
        hardModeEnabled = true;
    }

    public void Disable()
    {
        m_DestroyedEnemies = 0;
        m_HardModeCountTime = 0f;
        hardModeEnabled = false;
    }
}
