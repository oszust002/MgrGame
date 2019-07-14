using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AccelerationHandler
{
    public enum State
    {
        VERY_CALM, CALM, EXCITED
    }
    
    
    public readonly List<float> calibrationAccelerometerBuffer = new List<float>();    
    public readonly RollingList<float> accelerationBuffer;
    private readonly int m_ReadingFrequency;
    private float m_CalmJerkAbsMean;
    private readonly float m_MultiplyThreshold;
    private readonly float m_VeryCalmTimeThreshold;
    private State? m_LastState;
    private float m_LastCalmTime = -1;
    private float m_LastRead = -1;
    private readonly int m_SecondsBufferSize;

    public AccelerationHandler(int secondsBufferSize, int readingFrequency, float multiplyThreshold, float veryCalmTimeThreshold = 5)
    {
        m_SecondsBufferSize = secondsBufferSize;
        accelerationBuffer = new RollingList<float>(secondsBufferSize * readingFrequency);
        m_ReadingFrequency = readingFrequency;
        m_MultiplyThreshold = multiplyThreshold >= 1 ? multiplyThreshold : 1;
        m_VeryCalmTimeThreshold = veryCalmTimeThreshold;
    }

    public void CalculateCalibrationValues()
    {
        var jerk = CalculateJerk(calibrationAccelerometerBuffer.ToArray());
        m_CalmJerkAbsMean = jerk.Select(Math.Abs).Average();
    }

    public State GetCurrentState()
    {
        var jerk = CalculateJerk(accelerationBuffer.ToArray());
        var jerkAbsMean = jerk.Select(Math.Abs).Average();
        var currentState = jerkAbsMean > m_MultiplyThreshold * m_CalmJerkAbsMean ? State.EXCITED : State.CALM;
        if (currentState == State.CALM && WasCalmBefore() && IsCalmForLongTime())
        {
            currentState = State.VERY_CALM;
        }
        UpdateLastCalmTime(currentState);

        m_LastState = currentState;
        m_LastRead = Time.time;
        return currentState;
    }

    private void UpdateLastCalmTime(State currentState)
    {
        if (currentState == State.EXCITED)
        {
            m_LastCalmTime = -1;
        }
        else if (m_LastCalmTime < 0)
        {
            m_LastCalmTime = Time.time;
        }
        else if (!WasCalmBefore() || Time.time - m_LastRead > m_SecondsBufferSize)
        {
            m_LastCalmTime = Time.time;
        }
    }

    private bool IsCalmForLongTime()
    {
        if (m_LastCalmTime < 0) return false;
        if (Time.time - m_LastRead > m_SecondsBufferSize) return false;
        return Time.time - m_LastCalmTime > m_VeryCalmTimeThreshold;
    }

    private bool WasCalmBefore()
    {
        return m_LastState == State.CALM || m_LastState == State.VERY_CALM;
    }

    private List<float> CalculateJerk(float[] toArray)
    {
        var jerkValues = new List<float>();
        if (toArray.Length < 2) return jerkValues;

        for (var i = 0; i < toArray.Length - 1 ; i++)
        {
            var value = (toArray[i + 1] - toArray[i])/(1.0f / m_ReadingFrequency);
            jerkValues.Add(value);
        }

        return jerkValues;
    }
}