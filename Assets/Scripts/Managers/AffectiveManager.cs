using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AffectiveManager : MonoBehaviour
{
    public GameObject emotionManagerObject;
    [HideInInspector]
    public EmotionManager emotionManager;
    public GameObject sensorControllerObject;
    public GameObject loggerObject;
    [HideInInspector] public SensorController sensorController;
    public static AffectiveManager instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this);
        emotionManager = emotionManagerObject.GetComponent<EmotionManager>();
        sensorController = sensorControllerObject.GetComponent<SensorController>();
    }

    public void EnableAffectives()
    {
        emotionManagerObject.SetActive(true);
        sensorControllerObject.SetActive(true);
        loggerObject.SetActive(true);
    }

    public void DisableAffectives()
    {
        emotionManagerObject.SetActive(false);
        sensorControllerObject.SetActive(false);
    }

    public bool AffectiveEnabled()
    {
        return emotionManagerObject.activeSelf && sensorControllerObject.activeSelf;
    }

    public void StartCalibration()
    {
        emotionManager.StartCalibration();
        sensorController.StartCalibration();
    }

    public float GetCalibrationTime()
    {
        return Math.Max(emotionManager.calibrationTime, sensorController.FullCalibrationTime);
    }
}