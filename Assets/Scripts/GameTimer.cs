using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    private float maximumTimeLimit;
    private float remainingTime;
    [SerializeField] private MatchGameController matchGameControllerInstance;
    private bool timerIsActive;
    public bool hasTimerFinished;
    [SerializeField] private Image timerProgressBar;

    public void InitializeMaximumTime(int time)
    {
        maximumTimeLimit = time;
    }

    public void ResetTimer()
    {
        timerProgressBar.fillAmount = 1;
        remainingTime = maximumTimeLimit;
        timerIsActive = true;
        hasTimerFinished = false;
    }

    public void HaltTimer()
    {
        timerIsActive = false;
    }

    public void ResumeTimer()
    {
        timerIsActive = true;
    }
    
    public bool CheckTimerStatus()
    {
        return timerIsActive;
    }

    void Update()
    {
        if (timerIsActive)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                timerProgressBar.fillAmount = remainingTime / maximumTimeLimit;
            }
            else
            {
                ConcludeTimer();
            }
        }
    }

    private void ConcludeTimer()
    {
        remainingTime = 0;
        timerIsActive = false;
        hasTimerFinished = true;
        matchGameControllerInstance.ShowLoseScreen();
    }
}