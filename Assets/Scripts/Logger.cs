using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Logger : MonoBehaviour
{
    private const string LogDirPath = "./datalogs";
    private const string UserNumberPath = "./user_id.txt";
    public string userId = "0";

    private static Logger instance;

    private List<Event> events = new List<Event>();

    private long startTime;
    // Start is called before the first frame update

    private class Event
    {
        internal float time;
        internal string eventString;

        public Event(float time, string eventString)
        {
            this.time = time;
            this.eventString = eventString;
        }
    }
    
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        AffectiveManager.instance.emotionManager.onNewEmotion += LogEmotion;
        AffectiveManager.instance.sensorController.onThresholdPassed += OnThresholdPass;
        if (File.Exists(UserNumberPath))
        {
            var reader = new StreamReader(UserNumberPath);
            var id = reader.ReadLine();
            if (!string.IsNullOrEmpty(id))
            {
                userId = id;
            }
            reader.Close();
        }
    }

    private void OnEnable()
    {
        startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private void OnThresholdPass(float value)
    {
        var time = Time.time;
        var thresholdEvent = "EmgThresholdPass;" + value;
        events.Add(new Event(time, thresholdEvent));
    }

    private void LogEmotion(Emotion previousEmotion, Emotion emotion)
    {
        var time = Time.time;
        var emotionEvent = "Emotion;" + emotion.valence + ";" + emotion.arousal;
        if (!string.IsNullOrEmpty(emotion.additional))
        {
            emotionEvent += ";" + emotion.additional;
        }
        
        events.Add(new Event(time, emotionEvent));
    }
    
    private void OnDisable()
    {
        Save();
    }

    public void Save()
    {
        var logdir = LogDirPath + "/" + userId;
        Directory.CreateDirectory(logdir);
        var path = logdir + "/log_" + userId + "_" + Guid.NewGuid() + ".log";
        StreamWriter writer = File.AppendText(path);
        writer.WriteLine("UserID;" + userId);
        writer.WriteLine("StartTime;" + startTime);
        foreach (var e in events)
        {
            writer.WriteLine( e.time + ";" + e.eventString);
        }
        writer.Close();
    }
}
