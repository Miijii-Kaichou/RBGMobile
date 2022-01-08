using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OpposingPlayerTag : MonoBehaviour
{
    PlayerModelClientConnection referencePMCC;

    //UI
    [SerializeField]
    TextMeshProUGUI nameTMP;

    [SerializeField]
    TextMeshProUGUI scoreTMP;

    int _pID = 0;

    internal void SetUp(PlayerModelClientConnection playerModelClientConnection, int pID)
    {
        referencePMCC = playerModelClientConnection;
        if (referencePMCC != null)
        {
            _pID = pID; 
            DrawOnUI();
            return;
        }
    }

    void DrawOnUI()
    {
        nameTMP.text = string.Format("{0}: {1}",_pID, referencePMCC.playerName);
        UpdatePlayerScore();
    }

    void UpdatePlayerScore()
    {
        scoreTMP.text = referencePMCC.currentScore.ToString();
    }
}
