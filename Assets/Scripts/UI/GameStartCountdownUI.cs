using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countdownText;

    private void Start()
    {
        GameManager.Instance.OnStateChanged += OnStateChanged;
        Hide();
    }

    private void Update()
    {

        _countdownText.text = Mathf.Ceil(GameManager.Instance.GetCountdownToStartTimer()).ToString();

    }

    private void OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStartActive())
        {
            Show();
        }
        else
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
