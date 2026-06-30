using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] GameObject newGamePanel;
    private int existData;
    [SerializeField] Button loadButton;
    private void Start()
    {
        newGamePanel.SetActive(false);
        existData = PlayerPrefs.GetInt("ExistData" ,0);
        if (existData == 0)
        {
            loadButton.enabled = false;
        }
    }
    public void OnNewGameButtonClick()
    {
        if (existData == 0)
        {
            DataManager.instance.NewGame();
            SceneManager.LoadScene("Lobby");
        }
        else
        {
            newGamePanel.SetActive(true);
        }
    }
    public void OnLoadButtonClick()
    {
        DataManager.instance.LoadGameData();
        SceneManager.LoadScene("Lobby");
    }
    public void OnExitButtonClick()
    {
        Application.Quit();
    }
}
