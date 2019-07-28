using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ClassifierApiManager : MonoBehaviour
{
    private static string URI = "http://127.0.0.1:5000/classify";
    public HeartRateManager heartRateManager;
    private EmotionResponse m_LastEmotionResponse;
    public bool apiEnabled;
    
    [HideInInspector]
    public bool isNewEmotionSinceLastGet = false;

    [HideInInspector]
    public bool requestInProgress = false;

    public EmotionResponse GetLastEmotion()
    {
        isNewEmotionSinceLastGet = false;
        return m_LastEmotionResponse;
    }

    public IEnumerator AskForEmotion()
    {
        if (!apiEnabled)
        {
            yield break;
        }
        requestInProgress = true;
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

            requestInProgress = false;
        }
    }

    private void OnDisable()
    {
        apiEnabled = false;
    }
}