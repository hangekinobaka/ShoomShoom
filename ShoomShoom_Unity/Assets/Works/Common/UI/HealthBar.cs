using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Basics:")]
    [SerializeField] Slider _slider;
    [SerializeField] Gradient _gradient;
    [SerializeField] Image _fill;
    [SerializeField] CanvasGroup _canvasGroup;

    [Header("Optional:")]
    [SerializeField] bool _showAtBegin = true;
    [ConditionalDisplay("_showAtBegin", false)]
    [SerializeField] float _displayDuration = .8f;
    bool _isDisplaying = false;
    Coroutine _countDown;

    public void Init()
    {
        _canvasGroup.alpha = _showAtBegin ? 1 : 0;
    }

    public void SetMaxHealth(float health)
    {
        _slider.maxValue = health;
        _slider.value = health;
        // change color based on the damage value
        _fill.color = _gradient.Evaluate(1f);
    }

    public void SetHealth(float health)
    {
        _slider.value = health;
        // change color based on the damage value
        _fill.color = _gradient.Evaluate(_slider.normalizedValue);
    }

    public void DisplayHealthBar()
    {
        if (_showAtBegin) return;
        _canvasGroup.alpha = 1;
        _isDisplaying = true;

        if (_countDown != null) StopCoroutine(_countDown);
        _countDown = StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(_displayDuration);
        _canvasGroup.DOFade(endValue: 0f, duration: .2f)
            .OnComplete(() =>
            {
                _isDisplaying = false;
            });
    }
}