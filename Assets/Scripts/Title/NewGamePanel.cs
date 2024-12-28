using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGamePanel : MonoBehaviour
{
    public void OnConfirmButtonClick()
    {
        DataManager.instance.NewGame();
        SceneManager.LoadScene("Lobby");   

    }
    public void OnCancelButtonClick()
    {
        gameObject.SetActive(false);
    }
}
