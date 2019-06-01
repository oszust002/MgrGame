using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{
    public static Progress instance;
    
    public TextMeshProUGUI textScore;
    public Slider slider;
    
    private float m_Score;
    

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        m_Score = 0;
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        m_Score += amount;
        UpdateScoreUI();
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
