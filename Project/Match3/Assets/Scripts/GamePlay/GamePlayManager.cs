using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3Engine;
using UnityEngine.UI;

public class GamePlayManager : MonoSingleton<GamePlayManager>
{
    public delegate void GameOverDelegate();
    public event GameOverDelegate OnGameOver;
    public delegate void WinDelegate();
    public event WinDelegate OnWin;
    public delegate void LoseDelegate();
    public event LoseDelegate OnLose;
    public bool isSingleMod;

    float score;
    [SerializeField]
    int defaultSteps = 50;
    int steps = 0;
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text stepsText;
    [SerializeField]
    Text targetScoreText;
    [SerializeField]
    LevelConfig levelConfig = new LevelConfig();

    public void StartGame(int x, int y)
    {
        StartCoroutine(FieldManager.Instance.ClearField());
        if (isSingleMod)
        {
            targetScoreText.gameObject.SetActive(false);
            targetScoreText.text = "";
            steps = defaultSteps;
            ResetValues();
            FieldManager.Instance.Init(x, y);
        }
        else
        {
            targetScoreText.gameObject.SetActive(true);
            targetScoreText.text = levelConfig.scoreTarget.ToString();
            steps = levelConfig.steps;
            ResetValues();
            FieldManager.Instance.Init(levelConfig.size.x, levelConfig.size.y);
        }
    }

    public void StartNextLevel()
    {
        StartCoroutine(FieldManager.Instance.ClearField());
        levelConfig.steps += 8;
        levelConfig.size.x++;
        levelConfig.size.y++;
        levelConfig.scoreTarget += 500;
        steps = levelConfig.steps;
        targetScoreText.text = levelConfig.scoreTarget.ToString();
        ResetValues();
        FieldManager.Instance.Init(levelConfig.size.x, levelConfig.size.y);
    }

    public void RestartCurrentLevel()
    {
        StartCoroutine(FieldManager.Instance.ClearField());
        steps = levelConfig.steps;
        targetScoreText.text = levelConfig.scoreTarget.ToString();
        ResetValues();
        FieldManager.Instance.Init(levelConfig.size.x, levelConfig.size.y);
    }

    void ResetValues()
    {
        score = 0;
        stepsText.text = steps.ToString();
        scoreText.text = score.ToString();
    }

    public void AddScores(float score)
    {
        this.score += score;
        scoreText.text = this.score.ToString();
        if (!isSingleMod && this.score >= levelConfig.scoreTarget)
        {
            StartCoroutine(FieldManager.Instance.ClearField());
            OnWin();            
        }
    }

    public IEnumerator SpendStep()
    {
        while(FieldManager.Instance.IsFalling)
        {
            yield return null;
        }

        steps--;
        stepsText.text = steps.ToString();
        if(steps <= 0)
            TryEndGame();
    }
    
    public void AddStesp()
    {
        steps++;
    }

    void TryEndGame()
    {
        if (isSingleMod)
        {
            StartCoroutine(FieldManager.Instance.ClearField());
            OnGameOver();
        }
        else if (score < levelConfig.scoreTarget)
        {
            StartCoroutine(FieldManager.Instance.ClearField());
            OnLose();
        }
    }
}
