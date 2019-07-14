using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeartRateManager : MonoBehaviour
{
    private RollingList<HeartRateResponse> m_HeartRateResponses;
    public AntReader antReader;
    public int bufferSize = 80;
    public int readySize = 10;

    public bool IsReady => m_HeartRateResponses != null && m_HeartRateResponses.Count >= readySize;

    public double[] RrArray
    {
        get { return m_HeartRateResponses.Select(response => response.lastRr).ToArray(); }
    }

    public double[] HrArray
    {
        get { return m_HeartRateResponses.Select(response => (double) response.heartRate).ToArray(); }
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        m_HeartRateResponses = new RollingList<HeartRateResponse>(bufferSize);
        antReader.onNewHeartBeat -= SaveHeartBeat;
        antReader.onNewHeartBeat += SaveHeartBeat;
    }

    private void SaveHeartBeat(HeartRateResponse heartRateResponse)
    {
        if (heartRateResponse.IsFirstHeartBeat)
        {
            return;
        }

        m_HeartRateResponses.Add(heartRateResponse);
    }

    private void OnDisable()
    {
        antReader.onNewHeartBeat -= SaveHeartBeat;
    }
}
