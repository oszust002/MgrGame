using UnityEngine;
using UnityEngine.SceneManagement;

public class AffectiveManager : MonoBehaviour
{
    public GameObject emotionManagerObject;
    private EmotionManager m_EmotionManager;
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
        m_EmotionManager = emotionManagerObject.GetComponent<EmotionManager>();
        EnableAffectives();
    }

    public void EnableAffectives()
    {
        emotionManagerObject.SetActive(true);
    }

    public void DisableAffectives()
    {
        emotionManagerObject.SetActive(false);
    }
}