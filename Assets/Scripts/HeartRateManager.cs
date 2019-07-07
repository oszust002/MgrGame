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
    void Start()
    {
        m_HeartRateResponses = new RollingList<HeartRateResponse>(bufferSize);
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

    // Update is called once per frame
    void Update()
    {

    }

    public class RollingList<T> : IEnumerable<T>
    {
        private readonly LinkedList<T> _list = new LinkedList<T>();

        public RollingList(int maximumCount)
        {
            if (maximumCount <= 0)
                throw new ArgumentException(null, nameof(maximumCount));

            MaximumCount = maximumCount;
        }

        public int MaximumCount { get; }
        public int Count => _list.Count;

        public void Add(T value)
        {
            if (_list.Count == MaximumCount)
            {
                _list.RemoveFirst();
            }

            _list.AddLast(value);
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException();

                return _list.Skip(index).First();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
