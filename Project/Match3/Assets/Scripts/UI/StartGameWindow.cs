using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameWindow : MonoBehaviour
{
    int x;
    int y;

    public Toggle singleModToggle;
    public Button startButon;
    public Button exitButton;
    public InputField xField;
    public InputField yField;

    public GameObject setupItems;

    private void Start()
    {
        singleModToggle.isOn = false;
        GamePlayManager.Instance.OnGameOver += OnGameOverHandler;
        startButon.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(ExitGame);
        xField.onValueChanged.AddListener(OnFieldValueChanged);
        yField.onValueChanged.AddListener(OnFieldValueChanged);
        singleModToggle.onValueChanged.AddListener(SwitchMod);
        startButon.interactable = true;        
    }

    private void OnDestroy()
    {
        try
        {
            GamePlayManager.Instance.OnGameOver -= OnGameOverHandler;
            startButon.onClick.RemoveListener(StartGame);
            exitButton.onClick.RemoveListener(ExitGame);
            xField.onValueChanged.RemoveListener(OnFieldValueChanged);
            yField.onValueChanged.RemoveListener(OnFieldValueChanged);
            singleModToggle.onValueChanged.RemoveListener(SwitchMod);
        }
        catch(System.Exception ex)
        {
            Debug.LogWarning(ex, gameObject);
        }
    }

    bool ValidateInputs()
    {
        if (singleModToggle.isOn)
        {
            if (string.IsNullOrEmpty(xField.text) || string.IsNullOrEmpty(yField.text))
            {
                startButon.interactable = false;
                return false;
            }

            System.Int32.TryParse(xField.text, out x);
            System.Int32.TryParse(yField.text, out y);
            if (x < 3 || x > 15 || y < 3 || y > 15)
            {
                startButon.interactable = false;
                return false;
            }
            else
            {
                startButon.interactable = true;
                return true;
            }
        }
        else
        {
            startButon.interactable = true;
            return true;
        }
    }

    void OnFieldValueChanged(string str)
    {
        ValidateInputs();
    }

    void SwitchMod(bool value)
    {
        GamePlayManager.Instance.isSingleMod = value;
        setupItems.gameObject.SetActive(value);
        ValidateInputs();
    }

    void StartGame()
    {
        GamePlayManager.Instance.StartGame(x, y);
        gameObject.SetActive(false);
    }

    void ExitGame()
    {
        Application.Quit();
    }

    void OnGameOverHandler()
    {
        gameObject.SetActive(true);
    }
}
