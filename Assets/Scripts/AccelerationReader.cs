using System;
using Rewired;
using Rewired.ControllerExtensions;
using UnityEngine;

public class AccelerationReader : MonoBehaviour
{
    public int playerId = 0;
    [Range(20, 100)]
    public int frequency = 20;
    
    private Rewired.Player Player => ReInput.players.GetPlayer(playerId);
    private KalmanFilter m_KalmanFilter;

    public event OnNewRead onNewRead;
    [HideInInspector] public bool isWorking;
    [HideInInspector] public bool ds4Found;

    private void OnEnable()
    {
        var ds4 = GetFirstDs4(Player);
        if (ds4 == null)
        {
            ds4Found = true;
            return;
        }
        isWorking = false;
        m_KalmanFilter =  new KalmanFilter(0.21f, 0.01f,4);
        InvokeRepeating(nameof(ReadData), 0.5f, 1.0f/frequency);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(ReadData));
    }

    public delegate void OnNewRead(float accelerationMagnitude);


    private void ReadData()
    {
        if (!ReInput.isReady)
        {
            isWorking = false;
            return;
        }

        var ds4 = GetFirstDs4(Player);
        if (ds4 == null)
        {
            ds4Found = false;
            isWorking = false;
            return;
        }

        isWorking = true;
        var acc = ds4.GetAccelerometerValueRaw();
        var r = Math.Sqrt(acc.x * acc.x + acc.y * acc.y + acc.z * acc.z);
        var filteredMagnitude = m_KalmanFilter.UpdateState((float) r);
        onNewRead?.Invoke(filteredMagnitude);
    }

    private static IDualShock4Extension GetFirstDs4(Rewired.Player player)
    {
        foreach (var j in player.controllers.Joysticks)
        {
            // Use the interface because it works for both PS4 and desktop platforms
            var ds4 = j.GetExtension<IDualShock4Extension>();
            if (ds4 == null) continue;
            return ds4;
        }

        return null;
    }

    private class KalmanFilter
    {
        private float m_K;
        private float m_Q;
        private float m_R;
        private float m_P;
        private float? m_X;

        public KalmanFilter(float k, float q, float r)
        {
            m_K = k;
            m_Q = q;
            m_R = r;
            m_X = null;
        }

        public float UpdateState(float value)
        {
            if (!m_X.HasValue)
            {
                m_X = value;
                return value;
            }
            m_P = m_P + m_Q;
            m_K = m_P / (m_P + m_R);
            m_X = m_X + m_K * (value - m_X);
            m_P = (1 - m_K) * m_P;
            return m_X.Value;
        }
    }
}