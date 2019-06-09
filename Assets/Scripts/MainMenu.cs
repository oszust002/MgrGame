using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator introductionAnimator;
    private CanvasGroup canvasGroup;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        canvasGroup.alpha = 0;
        introductionAnimator.SetTrigger("Introduction");
        while (introductionAnimator.IsInTransition(0) || !GetCurrentAnimatorStateInfo().IsName("IntroductionAnimation") ||
               GetCurrentAnimatorStateInfo().IsName("IntroductionAnimation") && GetCurrentAnimatorStateInfo().normalizedTime < 1)
        {
            yield return null;
        }

        SceneManager.LoadScene("Level1");
    }

    private AnimatorStateInfo GetCurrentAnimatorStateInfo()
    {
        return introductionAnimator.GetCurrentAnimatorStateInfo(0);
    }
}