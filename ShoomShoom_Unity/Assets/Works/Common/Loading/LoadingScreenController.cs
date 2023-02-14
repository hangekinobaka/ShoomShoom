using DG.Tweening;
using UnityEngine;

public class LoadingScreenController : Singleton<LoadingScreenController>
{
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] float _defaultDuration = .3f;

    public float DefaultDuration => _defaultDuration;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Show()
    {
        _canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        _canvasGroup.alpha = 0;
    }

    public void FadeIn()
    {
        FadeIn(_defaultDuration);
    }
    public void FadeIn(float duration)
    {
        _canvasGroup.DOFade(1, duration);
    }

    public void FadeOut()
    {
        FadeOut(_defaultDuration);
    }
    public void FadeOut(float duration)
    {
        _canvasGroup.DOFade(0, duration);
    }
}
