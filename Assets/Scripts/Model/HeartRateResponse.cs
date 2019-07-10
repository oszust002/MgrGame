using System;

public class HeartRateResponse
{
    public readonly int heartRate;
    private readonly double m_HeartRateBeatCount;
    public readonly double lastHeartRateBeatTime;
    public readonly double lastRr;

    public ulong HeartRateBeatCount => (ulong) m_HeartRateBeatCount;

    public bool IsFirstHeartBeat => HeartRateBeatCount < 2;
    public double BeatTimeInMs => lastHeartRateBeatTime * 1000 / 1024;

    private HeartRateResponse(Builder builder)
    {
        heartRate = builder.heartRate;
        m_HeartRateBeatCount = builder.heartRateBeatCount;
        lastHeartRateBeatTime = builder.lastHeartRateBeatTime;
        lastRr = builder.lastRr;
    }

    public static Builder builder(int heartRate, double heartRateBeatCount, double lastHeartRateBeatTime)
    {
        return new Builder(heartRate, heartRateBeatCount, lastHeartRateBeatTime);
    }

    public override string ToString()
    {
        return "HR: " + heartRate +
               ", HR Beat Count: " + m_HeartRateBeatCount
               + ", Last HR Beat Time: " + lastHeartRateBeatTime
               + ", Last RR: " + lastRr;
    }
    
    
    public class Builder
    {
        public int heartRate;
        public double heartRateBeatCount;
        public double lastHeartRateBeatTime;
        public double lastRr;

        public Builder(int heartRate, double heartRateBeatCount, double lastHeartRateBeatTime)
        {
            this.heartRate = heartRate;
            this.heartRateBeatCount = heartRateBeatCount;
            this.lastHeartRateBeatTime = lastHeartRateBeatTime;
        }

        public Builder WithRr(double rr)
        {
            lastRr = rr;
            return this;
        }

        public HeartRateResponse Build()
        {
            return new HeartRateResponse(this);
        }
    }
}