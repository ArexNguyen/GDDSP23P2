using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    #region Unity_funcs
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region Scene_transitions
    public void StartGame()
    {
        Debug.Log("I got called");
        SceneManager.LoadScene("SampleScene");
    }

    public void LoseGame()
    {
        SceneManager.LoadScene("LoseScreen");
    }

    public void WinGame()
    {
        SceneManager.LoadScene("WinScreen");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }
    #endregion
}
