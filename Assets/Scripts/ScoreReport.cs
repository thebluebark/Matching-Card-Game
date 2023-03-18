using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreReport : MonoBehaviour
{
    private TMP_Text scoreReportText;

    private void Awake()
    {
        if (scoreReportText == null)
        {
            scoreReportText = GetComponent<TMP_Text>();
        }
    }

    void Start()
    {
        scoreReportText.text = "You have managed to find " + PlayerPrefs.GetInt("matches") + 
                                " matches within " + PlayerPrefs.GetInt("guesses") + " guesses.";
    }

}
