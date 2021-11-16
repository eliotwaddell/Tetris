using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour
{
    private int score;
    public TextMeshProUGUI scoretext;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        UpdateScore(0);
    }

    void Update()
    {
        UpdateScore(0);
    }

    public void UpdateScore(int scoreadd)
    {
        score += scoreadd;
        scoretext.SetText("Score: " + score);
    }

    public void Reset()
    {
        Start();
    }

}
