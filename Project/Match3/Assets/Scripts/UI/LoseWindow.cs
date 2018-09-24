using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseWindow : MonoBehaviour
{
    public GameObject parent;
    public Button restartButton;
    public Button menuButton;

    private void Start()
    {
        GamePlayManager.Instance.OnLose += OnLoseHandler;
        restartButton.onClick.AddListener(Restart);
        menuButton.onClick.AddListener(OpenMenu);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        try
        {
            GamePlayManager.Instance.OnLose -= OnLoseHandler;
            restartButton.onClick.RemoveListener(Restart);
            menuButton.onClick.RemoveListener(OpenMenu);
        }
        catch(System.Exception ex)
        {
            Debug.LogWarning(ex, gameObject);
        }
    }

    void OnLoseHandler()
    {
        gameObject.SetActive(true);
    }

    void Restart()
    {
        GamePlayManager.Instance.RestartCurrentLevel();
        gameObject.SetActive(false);
    }

    void OpenMenu()
    {
        parent.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
