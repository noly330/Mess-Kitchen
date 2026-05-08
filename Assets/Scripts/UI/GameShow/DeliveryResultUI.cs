using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System;

public class DeliveryResultUI : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private Color _successColor;
    [SerializeField] private Color _failedColor;
    [SerializeField] private Sprite _successIcon;
    [SerializeField] private Sprite _failedIcon;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        gameObject.SetActive(false);
    }


    private void DeliveryManager_OnRecipeSuccess(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        _animator.SetTrigger(AnimatorHash.Popup);
        _backgroundImage.color = _successColor;
        _iconImage.sprite = _successIcon;
        _messageText.text = "DELIVERY\nSUCCESS";
    }
    private void DeliveryManager_OnRecipeFailed(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        _animator.SetTrigger(AnimatorHash.Popup);
        _backgroundImage.color = _failedColor;
        _iconImage.sprite = _failedIcon;
        _messageText.text = "DELIVERY\nFAILED";
    }
}
