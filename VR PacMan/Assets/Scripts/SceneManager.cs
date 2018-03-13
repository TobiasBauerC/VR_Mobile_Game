using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance = null;


	// Use this for initialization
	void Start () 
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
	}

    public void GoToMainMenu(int mainMenuIndex)
    {
        LoadScene(mainMenuIndex);
    }

    public void GoToLevel(int levelIndex)
    {
        LoadScene(levelIndex);
    }

    public void GoToGameOver(int gameOverIndex)
    {
        LoadScene(gameOverIndex);
    }

    private void LoadScene(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }
}
