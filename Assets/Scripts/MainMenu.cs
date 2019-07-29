using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator introductionAnimator;
    public TextMeshProUGUI affectiveIntroductionText;
    public GameObject affectiveIntroduction;
    public TextMeshProUGUI waitingText;
    private Animator affectiveIntroductionAnimator;
    private CanvasGroup canvasGroup;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        affectiveIntroductionAnimator = affectiveIntroduction.GetComponent<Animator>();
        
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        canvasGroup.alpha = 0;
        if (!Options.affectiveEnabled)
        {
            introductionAnimator.SetTrigger("Introduction");
            while (introductionAnimator.IsInTransition(0) ||
                   !GetCurrentAnimatorStateInfo(introductionAnimator).IsName("IntroductionAnimation") ||
                   GetCurrentAnimatorStateInfo(introductionAnimator).IsName("IntroductionAnimation") &&
                   GetCurrentAnimatorStateInfo(introductionAnimator).normalizedTime < 1)
            {
                yield return null;
            }
        }
        else
        {
            //Maybe move to options and give feedback if everything is working?
            AffectiveManager.instance.EnableAffectives();
            
            //Introduction phase
            var calibrationTime = AffectiveManager.instance.GetCalibrationTime();
            affectiveIntroductionText.text = affectiveIntroductionText.text.Replace("{}", calibrationTime.ToString());
            affectiveIntroductionAnimator.SetTrigger("Introduction");
            yield return new WaitForSeconds(10);
            while (affectiveIntroductionAnimator.IsInTransition(0) ||
                   !GetCurrentAnimatorStateInfo(affectiveIntroductionAnimator).IsName("AffectiveIntroduction") ||
                   GetCurrentAnimatorStateInfo(affectiveIntroductionAnimator).IsName("AffectiveIntroduction") &&
                   GetCurrentAnimatorStateInfo(affectiveIntroductionAnimator).normalizedTime < 1)
            {
                yield return null;
            }
            
            //Calibration phase
            affectiveIntroduction.SetActive(false);
            AffectiveManager.instance.StartCalibration();
            var timePassed = 0f;
            waitingText.gameObject.SetActive(true);
            while (timePassed < calibrationTime || AffectiveManager.instance.emotionManager.calibrationPhase)
            {
                yield return new WaitForSeconds(0.5f);
                timePassed += 0.5f;
                SetWaitText(timePassed);
            }
        }

        SceneManager.LoadScene("Level1");
    }

    private void SetWaitText(float timePassed)
    {
        var repeats = (int) (timePassed / 0.5) % 3;
        waitingText.text = new string('.', repeats + 1);
    }

    private AnimatorStateInfo GetCurrentAnimatorStateInfo(Animator animator)
    {
        return animator.GetCurrentAnimatorStateInfo(0);
    }
}