using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem.iOS;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private Image _timerImage;

    private void Update()
    {
        _timerImage.fillAmount = GameManager.Instance.GetGamePlayingTimerNormalized();
    }
}
