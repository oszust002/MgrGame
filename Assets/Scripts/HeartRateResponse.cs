using System;

public class HeartRateResponse
{
    public readonly int heartRate;
    public readonly double heartRateBeatCount;
    public readonly double lastHeartRateBeatTime;
    public readonly double lastRr;
    private readonly double m_RmssdHrvSquared;
    public double RmssdHrv => Math.Sqrt(m_RmssdHrvSquared);
    public double BeatTimeInMs => lastHeartRateBeatTime * 1000 / 1024;

    private HeartRateResponse(Builder builder)
    {
        heartRate = builder.heartRate;
        heartRateBeatCount = builder.heartRateBeatCount;
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
               ", HR Beat Count: " + heartRateBeatCount
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

        public Builder WithRmssdSquared(double rmssdHRVSquared)
        {
            this.rmssdHrvSquared = rmssdHRVSquared;
            return this;
        }

        public HeartRateResponse Build()
        {
            return new HeartRateResponse(this);
        }
    }
}