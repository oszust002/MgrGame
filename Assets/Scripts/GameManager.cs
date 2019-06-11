using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public Animator gameOverAnimator;
    public static GameManager instance;
    public GameObject gameOverInstructions;

    public static bool gameEnded;
    public static bool gamePaused;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (gameEnded)
        {
            if (Input.GetButton("Fire1"))
            {
                gameEnded = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else if (Input.GetButton("Cancel"))
            {
                gameEnded = false;
                SceneManager.LoadScene("MainMenu");
            }
            return;
        }

        if (Input.GetButton("Cancel"))
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                return;
            }

            gamePaused = !gamePaused;
        }
    }

    // Update is called once per frame
    public void PlayerDied()
    {
        StartCoroutine(PlayerDeath());
    }

    private IEnumerator PlayerDeath()
    {
        
        gameOverAnimator.SetTrigger("GameOver");
        yield return new WaitForSeconds(1.5f);
        gameEnded = true;
        
    }
}
