using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForOnterPlayerUI : MonoBehaviour
{

    private void Start()
    {
        GameManager.Instance.OnLoaclPlayerReadyChanged += GameManager_OnLoaclPlayerReadyChanged;
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }


    private void GameManager_OnLoaclPlayerReadyChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerReady)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStartActive())
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
