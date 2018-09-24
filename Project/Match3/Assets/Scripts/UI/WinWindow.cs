using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinWindow : MonoBehaviour
{
    public GameObject parent;
    public Button nextLvltButon;
    public Button menuButton;

    private void Start()
    {        
        GamePlayManager.Instance.OnWin += OnWinHandler;
        nextLvltButon.onClick.AddListener(StartNextLvl);
        menuButton.onClick.AddListener(OpenMenu);
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        try
        {
            GamePlayManager.Instance.OnWin -= OnWinHandler;
            nextLvltButon.onClick.RemoveListener(StartNextLvl);
            menuButton.onClick.RemoveListener(OpenMenu);
        }
        catch(System.Exception ex)
        {
            Debug.LogWarning(ex, gameObject);
        }        
    }    

    void OnWinHandler()
    {
        gameObject.SetActive(true);
    }

    void StartNextLvl()
    {
        GamePlayManager.Instance.StartNextLevel();
        gameObject.SetActive(false);
    }

    void OpenMenu()
    {
        parent.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
