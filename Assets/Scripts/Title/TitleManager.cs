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
        existData = PlayerPrefs.GetInt("ExistData" ,0);//0은 로드할 수 있는 데이터가 없음, 1은 있음
        if (existData == 0)
        {
            loadButton.enabled = false;
        }
    }
    public void OnNewGameButtonClick()
    {
        if (existData == 0)//불러올 데이터가 없다면
        {
            DataManager.instance.NewGame();
            SceneManager.LoadScene("Lobby");
        }
        else//불러올 데이터가 있다면
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
