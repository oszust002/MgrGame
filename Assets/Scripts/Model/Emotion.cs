using System.Text.RegularExpressions;

public class Emotion
{
    private static readonly Regex emotionRegex = new Regex(@"v(.)a(.)");

    public enum Level
    {
        LOW, MEDIUM, HIGH
    }

    public Level valence;
    public Level arousal;

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
        if (state != AccelerationHandler.State.EXCITED) return;
        Level newArousal = arousal;
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
        arousal = newArousal;

    }
}