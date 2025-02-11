using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthIndicator : MonoBehaviour, IIndicator
{
    [SerializeField] private Image _indicatorImage;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField, InspectorVariantName("値が更新されない際に表示を消すまでの時間(秒)")] private float _hideIndicatorTime = 0.5f;
    private bool _isVisible = true;
    private float _timer;
    private Tween _currentTween;
    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_isVisible && _timer <= 0f)
        {
            HideIndicator();
        }
    }

    private void HideIndicator()
    {
        _isVisible = false;
        _currentTween?.Kill();
        _currentTween = _canvasGroup.DOFade(0f, 0.5f).SetLink(gameObject);
    }

    private void ShowIndicator()
    {
        _isVisible = true;
        _currentTween?.Kill();
        _currentTween = _canvasGroup.DOFade(1f, 0.5f).SetLink(gameObject);
    }

    public void SetRatio(float ratio)
    {
        _indicatorImage.fillAmount = ratio;
        _timer = _hideIndicatorTime;
        if (!_isVisible) ShowIndicator();
    }
}