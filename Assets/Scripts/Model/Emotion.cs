using System.Text.RegularExpressions;

public class Emotion
{
    public static readonly Emotion Neutral = new Emotion(Level.MEDIUM, Level.MEDIUM);
    public static readonly Emotion Sad = new Emotion(Level.LOW, Level.LOW);
    public static readonly Emotion Angry = new Emotion(Level.LOW, Level.MEDIUM);
    public static readonly Emotion Scared = new Emotion(Level.LOW, Level.HIGH);
    public static readonly Emotion Tired = new Emotion(Level.MEDIUM, Level.LOW);
    public static readonly Emotion Surprised = new Emotion(Level.MEDIUM, Level.HIGH);
    public static readonly Emotion Relaxed = new Emotion(Level.HIGH, Level.LOW);
    public static readonly Emotion Happy = new Emotion(Level.HIGH, Level.MEDIUM);
    public static readonly Emotion Excited = new Emotion(Level.HIGH, Level.HIGH);
    
    private static readonly Regex emotionRegex = new Regex(@"v(.)_a(.)");


    public enum Level
    {
        LOW, MEDIUM, HIGH
    }

    public Level valence;
    public Level arousal;
    public string additional = "";


    private Emotion(Level valence, Level arousal)
    {
        this.valence = valence;
        this.arousal = arousal;
    }

    public Emotion(EmotionResponse emotionResponse)
    {
        if (emotionResponse?.emotion == null)
        {
            valence = Level.MEDIUM;
            arousal = Level.MEDIUM;
            return;
        }
        var match = emotionRegex.Match(emotionResponse.emotion);
        if (!match.Success)
        {
            valence = Level.MEDIUM;
            arousal = Level.MEDIUM;
            return;
        }
        valence = GetLevelFromString(match.Groups[1].Value);
        arousal = GetLevelFromString(match.Groups[2].Value);
        additional = emotionResponse.additional;
    }

    private static Level GetLevelFromString(string levelString)
    {
        switch (levelString)
        {
            case "L": return Level.LOW;
            case "M": return Level.MEDIUM;
            case "H": return Level.HIGH;
            default: return Level.MEDIUM;
        }
    }

    public void Tune(AccelerationHandler.State state)
    {
        Level newArousal = arousal;
        if (state == AccelerationHandler.State.EXCITED)
        {
            switch (arousal)
            {
                case Level.LOW:
                {
                    newArousal = Level.MEDIUM;
                    break;
                }
                case Level.MEDIUM:
                {
                    newArousal = Level.HIGH;
                    break;
                }
                case Level.HIGH:
                {
                    newArousal = Level.HIGH;
                    break;
                }
            }
        }
        else if (state == AccelerationHandler.State.VERY_CALM)
        {
            switch (arousal)
            {
                case Level.LOW:
                {
                    newArousal = Level.LOW;
                    break;
                }
                case Level.MEDIUM:
                {
                    newArousal = Level.LOW;
                    break;
                }
                case Level.HIGH:
                {
                    newArousal = Level.MEDIUM;
                    break;
                }
            }
        }

        arousal = newArousal;

    }

    private bool Equals(Emotion other)
    {
        return valence == other.valence && arousal == other.arousal;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Emotion) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((int) valence * 397) ^ (int) arousal;
        }
    }

    public override string ToString()
    {
        return "Valence: " + valence + ", Arousal: " + arousal;
    }
}