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
    public float jerkThresholdMultiplier;

    private AccelerationHandler m_AccelerationHandler;
    private bool m_ReadCalibrationValues = true;

    public bool calibrationPhase = true;
    
    private void Start()
    {
        m_AccelerationHandler = new AccelerationHandler(
            accelerationReader.frequency * accelerationSecondsBufferSize, 
            accelerationReader.frequency, jerkThresholdMultiplier);
        accelerationReader.onNewRead += HandleNewAcceleration;
        StartCoroutine(Calibrate());
    }

    private IEnumerator Calibrate()
    {
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

    private void HandleNewAcceleration(float accelerationmagnitude)
    {
        if (m_ReadCalibrationValues && calibrationPhase)
        {
            m_AccelerationHandler.calibrationAccelerometerBuffer.Add(accelerationmagnitude);
        }
        m_AccelerationHandler.accelerationBuffer.Add(accelerationmagnitude);
    }
}