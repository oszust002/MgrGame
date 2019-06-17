using System;

public class HeartRateResponse
{
    public readonly int heartRate;
    private readonly double m_HeartRateBeatCount;
    public readonly double lastHeartRateBeatTime;
    public readonly double lastRr;
    private readonly double m_RmssdHrvSquared;
    public double RmssdHrv => Math.Sqrt(m_RmssdHrvSquared);

    public ulong HeartRateBeatCount => (ulong) m_HeartRateBeatCount;

    public bool IsFirstHeartBeat => HeartRateBeatCount < 2;
    public double BeatTimeInMs => lastHeartRateBeatTime * 1000 / 1024;

    private HeartRateResponse(Builder builder)
    {
        heartRate = builder.heartRate;
        m_HeartRateBeatCount = builder.heartRateBeatCount;
        lastHeartRateBeatTime = builder.lastHeartRateBeatTime;
        lastRr = builder.lastRr;
        m_RmssdHrvSquared = builder.rmssdHrvSquared;
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
               + ", Last RR: " + lastRr 
               + ", Last RMSSD HRV: " + RmssdHrv;
    }
    
    
    public class Builder
    {
        public int heartRate;
        public double heartRateBeatCount;
        public double lastHeartRateBeatTime;
        public double lastRr;
        public double rmssdHrvSquared;

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

        public Builder WithRmssdSquared(double rmssdHrvSquared)
        {
            this.rmssdHrvSquared = rmssdHrvSquared;
            return this;
        }

        public HeartRateResponse Build()
        {
            return new HeartRateResponse(this);
        }
    }
}