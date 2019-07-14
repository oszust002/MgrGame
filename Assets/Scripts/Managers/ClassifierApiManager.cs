using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ClassifierApiManager : MonoBehaviour
{
    private static string URI = "http://localhost:5000/classify";
    public HeartRateManager heartRateManager;
    public float emotionAskTime = 5;
    private float m_Time = 0;
    private EmotionResponse m_LastEmotionResponse;
    public bool apiEnabled;
    
    [HideInInspector]
    public bool isNewEmotionSinceLastGet = false;

    private void OnEnable()
    {
        m_Time = emotionAskTime;
    }

    private void Update()
    {
        if (!apiEnabled)
        {
            return;
        }
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

    public EmotionResponse GetLastEmotion()
    {
        isNewEmotionSinceLastGet = false;
        return m_LastEmotionResponse;
    }

    private IEnumerator AskForEmotion()
    {
        var rrArray = heartRateManager.RrArray.Select(rr => rr/1000).ToArray();
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

            if (!req.isHttpError && !req.isNetworkError && req.error == null)
            {
                var result = req.downloadHandler.data;
                var json = System.Text.Encoding.Default.GetString(result);
                var emotion = JsonUtility.FromJson<EmotionResponse>(json);
                m_LastEmotionResponse = emotion;
                isNewEmotionSinceLastGet = true;
            }
        }
    }

    private void OnDisable()
    {
        apiEnabled = false;
    }
}