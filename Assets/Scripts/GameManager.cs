using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Animator gameOverAnimator;
    public static GameManager instance;
    public GameObject pauseMenu;

    public bool gameEnded;
    public bool gamePaused;

    // Start is called before the first frame update
    void Awake()
    {
//        GameObject.FindGameObjectWithTag()
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
                ExitToMenu();
            }

            return;
        }

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            return;
        }

        if (!gamePaused && Input.GetButtonDown("Options"))
        {
            Pause();
        }
        else if (gamePaused && (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Options")))
        {
            if (gamePaused)
            {
                Resume();
            }
        }
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        gamePaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        gamePaused = false;
    }

    public void ExitToMenu()
    {
        AffectiveManager.instance.DisableAffectives();
        SceneManager.LoadScene("MainMenu");
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