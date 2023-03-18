using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("CardGame");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
