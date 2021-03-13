using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Board board;
    [SerializeField] protected UIManager ui;
    static float bestScore;
    private int score = 0;
    private float multiplier;
    private int nbTurns = 0;

    private System.DateTime startTime;

    protected virtual void Awake()
    {
        board.OnStartPlayerTurn += PlayerTurnStart;
        board.OnPlayerSelectedCell += PlayerTurnEnd;
        board.OnEndLevel += EndLevelReached;
    }

    private void PlayerTurnStart()
    {
        startTime = System.DateTime.Now;
        //Debug.Log($"PlayerTurnStart {startTime}");
    }

    private void PlayerTurnEnd()
    {
        nbTurns++;
        System.DateTime endTime = System.DateTime.Now;
        System.TimeSpan delta = (endTime - startTime);
        int val = Score((float) delta.TotalSeconds);
        //Debug.Log($"PlayerTurnEnd {endTime}, {delta.TotalSeconds}, {val}");
        score += val;
    }


    private void EndLevelReached()
    {
        score += Score(nbTurns);
    }

    public int Score(float delta)
    {
        return (int)( 100.0f * Mathf.Exp(-0.1f * delta)); // 100*e^(-0.1x)
    }

    private void Update()
    {
        ui.DisplayScore(score);
    }

    private void OnDestroy()
    {
        board.OnStartPlayerTurn += PlayerTurnStart;
        board.OnPlayerSelectedCell += PlayerTurnEnd;
        board.OnEndLevel += EndLevelReached;
    }
}
