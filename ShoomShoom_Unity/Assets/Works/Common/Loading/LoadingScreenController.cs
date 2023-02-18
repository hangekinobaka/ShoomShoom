using DG.Tweening;
using UnityEngine;

public class LoadingScreenController : Singleton<LoadingScreenController>
{
    [SerializeField] GameObject _canvas;
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] float _defaultDuration = .3f;

    public float DefaultDuration => _defaultDuration;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Show()
    {
        _canvas.SetActive(true);
        _canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        _canvas.SetActive(false);
        _canvasGroup.alpha = 0;
    }

    public void FadeIn()
    {
        FadeIn(_defaultDuration);
    }
    public void FadeIn(float duration)
    {
        _canvasGroup.DOFade(1, duration)
            .OnComplete(() =>
            {
                _canvas.SetActive(true);
            });
    }

    public void FadeOut()
    {
        FadeOut(_defaultDuration);
    }
    public void FadeOut(float duration)
    {
        _canvas.SetActive(true);
        _canvasGroup.DOFade(0, duration)
        .OnComplete(() =>
        {
            _canvas.SetActive(false);
        });
    }
}
