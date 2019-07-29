using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ClassifierApiManager : MonoBehaviour
{
    private static string URI = "http://127.0.0.1:5000";
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

    private void Start()
    {
        string[] args = Environment.GetCommandLineArgs();
        int portArgIndex = Array.FindIndex(args, x => x == "--classifier-url" || x == "-clu");
        if (portArgIndex != -1 && args.Length > portArgIndex)
        {
            URI = args[portArgIndex + 1];
        }
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
        
        var uri = URI + (URI.EndsWith("/") ? "classify" : "/classify");
        using (UnityWebRequest req = UnityWebRequest.Put(uri,JsonUtility.ToJson(requestBody)))
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