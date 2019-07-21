using System;
using System.Collections;
using UnityEngine;

public class EmotionManager : MonoBehaviour
{
    [Header("Managers")]
    public ClassifierApiManager classifierApiManager;
    public AccelerationReader accelerationReader;
    
    [Header("Parameters")]
    [Min(10)]
    public float calibrationTime = 20;
    public int accelerationSecondsBufferSize = 4;
    [Range(1,10)]
    public float jerkThresholdMultiplier = 4;

    private AccelerationHandler m_AccelerationHandler;
    private bool m_ReadCalibrationValues = true;

    [HideInInspector]
    public bool calibrationPhase = true;

    public float emotionAskTime = 5f;
    private float time = 0;

    public event OnNewEmotion onNewEmotion;

    public delegate void OnNewEmotion(Emotion emotion);
    private void OnEnable()
    {
        time = emotionAskTime;
        accelerationReader.onNewRead -= HandleNewAcceleration;
        m_ReadCalibrationValues = true;
        calibrationPhase = true;
        m_AccelerationHandler = new AccelerationHandler(accelerationSecondsBufferSize, 
            accelerationReader.frequency, jerkThresholdMultiplier);
    }

    private void Update()
    {
        if (calibrationPhase)
        {
            return;
        }

        if (time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {
            time = emotionAskTime;
            StartCoroutine(AskForEmotion());
        }
        
    }

    private IEnumerator AskForEmotion()
    {
        if (classifierApiManager.heartRateManager.IsReady)
        {
            StartCoroutine(classifierApiManager.AskForEmotion());
            while (classifierApiManager.requestInProgress)
            {
                yield return null;
            }
        }

        var emotion = GetEmotion();
        if (emotion != null)
        {
            onNewEmotion?.Invoke(emotion);
        }
    }

    public void StartCalibration()
    {
        classifierApiManager.apiEnabled = true;
        accelerationReader.onNewRead -= HandleNewAcceleration;
        accelerationReader.onNewRead += HandleNewAcceleration;
        StartCoroutine(Calibrate());
    }

    private IEnumerator Calibrate()
    {
        if (accelerationReader.ds4Found)
        {
            yield break;
        }
        var timeLeft = calibrationTime;
        while (timeLeft > 0)
        {
            if (accelerationReader.isWorking)
            {
                timeLeft -= 0.2f;
            }

            yield return new WaitForSeconds(0.2f);
        }

        m_ReadCalibrationValues = false;

        m_AccelerationHandler.CalculateCalibrationValues();
        calibrationPhase = false;
    }

    private Emotion GetEmotion()
    {
        var emotion = new Emotion(classifierApiManager.GetLastEmotion());
        if (calibrationPhase)
        {
            return emotion;
        }
        emotion.Tune(m_AccelerationHandler.GetCurrentState());
        return emotion;
    }

    private void HandleNewAcceleration(float accelerationmagnitude)
    {
        if (m_ReadCalibrationValues && calibrationPhase)
        {
            m_AccelerationHandler.calibrationAccelerometerBuffer.Add(accelerationmagnitude);
        }
        m_AccelerationHandler.accelerationBuffer.Add(accelerationmagnitude);
    }

    private void OnDisable()
    {
        accelerationReader.onNewRead -= HandleNewAcceleration;
    }
}