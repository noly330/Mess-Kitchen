
using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image _barImage;
    private IHasProgress _hasProgress;


    private void Awake()
    {
        _hasProgress = GetComponentInParent<IHasProgress>();
    }

    private void Start()
    {
        _hasProgress.OnProgressChanged += OnProgressChanged;
        _barImage.fillAmount = 0;
        Hide();
    }

    private void OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        _barImage.fillAmount = e.progressNormalized;
        if (e.progressNormalized >= 1 || e.progressNormalized <= 0)
        {
            Hide();
        }
        else
        {
            Show();
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
