using UnityEngine;
using UnityEngine.SceneManagement;

public class AffectiveManager : MonoBehaviour
{
    public GameObject emotionManagerObject;
    [HideInInspector]
    public EmotionManager emotionManager;
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
    }

    public void EnableAffectives()
    {
        emotionManagerObject.SetActive(true);
    }

    public void DisableAffectives()
    {
        emotionManagerObject.SetActive(false);
    }

    public bool AffectiveEnabled()
    {
        return emotionManagerObject.activeSelf;
    }
}