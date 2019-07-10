using System;
using System.Collections.Generic;
using System.Linq;

public class AccelerationHandler
{
    public enum State
    {
        CALM, EXCITED
    }
    
    
    public List<float> calibrationAccelerometerBuffer = new List<float>();
    public RollingList<float> accelerationBuffer;
    private int m_ReadingFrequency;
    private float m_CalmJerkAbsMean;
    private float m_MultiplyThreshold;

    public AccelerationHandler(int bufferSize, int readingFrequency, float multiplyThreshold)
    {
        accelerationBuffer = new RollingList<float>(bufferSize);
        m_ReadingFrequency = readingFrequency;
        m_MultiplyThreshold = multiplyThreshold >= 1 ? multiplyThreshold : 1;
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
        return jerkAbsMean > m_MultiplyThreshold * m_CalmJerkAbsMean ? State.EXCITED : State.CALM;
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