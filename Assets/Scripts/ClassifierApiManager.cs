using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ClassifierApiManager : MonoBehaviour
{
    private static string URI = "http://localhost:5000/classify";
    public HeartRateManager heartRateManager;
    public float emotionAskTime = 5;
    private float m_Time = 0;
    private Emotion m_LastEmotion;
    
    [HideInInspector]
    public bool isNewEmotionSinceLastGet = false;

    private void Start()
    {
        m_Time = emotionAskTime;
    }

    private void Update()
    {
        if (m_Time >= 0)
        {
            m_Time -= Time.deltaTime;
        }
        else if (heartRateManager.IsReady)
        {
            m_Time = emotionAskTime;
            StartCoroutine(AskForEmotion());
        }
    }

    public Emotion GetLastEmotion()
    {
        isNewEmotionSinceLastGet = false;
        return m_LastEmotion;
    }

    private IEnumerator AskForEmotion()
    {
        var rrArray = heartRateManager.RrArray;
        var hrArray = heartRateManager.HrArray;
        var requestBody = new RequestBody {hr = hrArray, rr = rrArray};
        
        using (UnityWebRequest req = UnityWebRequest.Put(URI,JsonUtility.ToJson(requestBody)))
        {
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();
            while (!req.isDone)
            {
                yield return null;
            }
            var result = req.downloadHandler.data;
            var json = System.Text.Encoding.Default.GetString(result);
            var emotion = JsonUtility.FromJson<Emotion>(json);
            m_LastEmotion = emotion;
            isNewEmotionSinceLastGet = true;
        }
    }
}